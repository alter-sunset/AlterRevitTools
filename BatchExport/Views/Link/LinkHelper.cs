using System.IO;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AlterTools.BatchExport.Utils;
using Autodesk.Revit.Exceptions;

namespace AlterTools.BatchExport.Views.Link
{
    internal static class LinkHelper
    {
        private const string DiffCoord = "Обнаружено различие систем координат. Выполнить получение коордианат из файла?";
        internal static void CreateLinks(this LinkViewModel linkViewModel, UIApplication uiApp)
        {
            Document doc = uiApp.ActiveUIDocument.Document;

            bool isCurrentWorkset = linkViewModel.IsCurrentWorkset;
            bool setWorksetId = !isCurrentWorkset && (0 < linkViewModel.Worksets.Length);

            List<Entry> entries = linkViewModel.Entries.Where(entry => !string.IsNullOrWhiteSpace(entry.Name) && File.Exists(entry.Name))
                                                       .OrderBy(entry => entry.SelectedWorkset?.Name ?? string.Empty)
                                                       .ToList();

            LinkProps props = new(doc.GetWorksetTable(), setWorksetId, linkViewModel.PinLinks, linkViewModel.WorksetPrefixes);

            entries.ForEach(entry => TryCreateLink(doc, entry, props));
        }
        private static void TryCreateLink(Document doc, Entry entry, LinkProps props)
        {
            BasicFileInfo fileInfo = BasicFileInfo.Extract(entry.Name);

            ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(entry.Name);

            RevitLinkOptions revitLinkOptions = fileInfo.IsWorkshared && (0 != props.WorksetPrefixes.Length)
                ? new RevitLinkOptions(false, linkPath.CloseWorksets(props.WorksetPrefixes))
                : new RevitLinkOptions(false);

            using Transaction tr = new(doc);

            tr.Start($"Link {entry.Name}");

            if (props.SetWorksetId)
            {
                props.WorksetTable.SetActiveWorksetId(entry.SelectedWorkset.Id);
            }

            RevitLinkInstance revitLinkInstance;
            LinkLoadResult linkLoadResult = null;

            try
            {
                linkLoadResult = RevitLinkType.Create(doc, linkPath, revitLinkOptions);
                revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, entry.SelectedImportPlacement);
                revitLinkInstance.Pinned = props.PinLink;

                tr.Commit();
            }
            catch (InvalidOperationException)
            {
                if (null == linkLoadResult)
                {
                    tr.RollBack();
                    return;
                }
                
                revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);
                ModelHelper.YesNoTaskDialog(DiffCoord, () => doc.AcquireCoordinates(revitLinkInstance.Id));
                revitLinkInstance.Pinned = props.PinLink;

                tr.Commit();
            }
            catch
            {
                tr.RollBack();
            }
        }

        private static WorksetConfiguration CloseWorksetsWithLinks(ModelPath modelPath, string[] prefixes)
        {
            if (null == prefixes 
                || 0 == prefixes.Length) return new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);

            // problem occurs if centralModel can't be found
            try
            {
                WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.CloseAllWorksets);
                
                IList<WorksetId> worksetIds = WorksharingUtils.GetUserWorksetInfo(modelPath)
                    .Where(wp => !prefixes.Any(wp.Name.StartsWith))
                    .Select(wp => wp.Id)
                    .ToList();
                
                worksetConfiguration.Open(worksetIds);
                
                return worksetConfiguration;
            }
            catch
            {
                // just return default worksetConfiguration
                return new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);
            }
        }
    }
}