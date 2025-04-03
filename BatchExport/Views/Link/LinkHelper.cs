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
        private const string DIFF_COORD = "Обнаружено различие систем координат. Выполнить получение коордианат из файла?";
        internal static void CreateLinks(this LinkViewModel linkViewModel, UIApplication uiApp)
        {
            Document doc = uiApp.ActiveUIDocument.Document;
            bool isCurrentWorkset = linkViewModel.IsCurrentWorkset;
            List<Entry> entries = linkViewModel.Entries
                .Where(e => !string.IsNullOrEmpty(e.Name) && File.Exists(e.Name))
                .OrderBy(e => e.SelectedWorkset?.Name ?? string.Empty)
                .ToList();

            bool setWorksetId = !isCurrentWorkset && linkViewModel.Worksets.Length > 0;
            LinkProps props = new(doc.GetWorksetTable(), setWorksetId, linkViewModel.PinLinks, linkViewModel.WorksetPrefixes);

            entries.ForEach(entry => TryCreateLink(doc, entry, props));
        }
        private static void TryCreateLink(Document doc, Entry entry, LinkProps props)
        {
            BasicFileInfo fileInfo = BasicFileInfo.Extract(entry.Name);
            ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(entry.Name);

            RevitLinkOptions revitLinkOptions = fileInfo.IsWorkshared && props.WorksetPrefixes.Length != 0
                ? new(false, CloseWorksetsWithLinks(linkPath, props.WorksetPrefixes))
                : new(false);

            using Transaction t = new(doc);
            t.Start($"Link {entry.Name}");

            if (props.SetWorksetId)
                props.WorksetTable.SetActiveWorksetId(entry.SelectedWorkset.Id);

            RevitLinkInstance revitLinkInstance;
            LinkLoadResult linkLoadResult = default;

            try
            {
                linkLoadResult = RevitLinkType.Create(doc, linkPath, revitLinkOptions);
                revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, entry.SelectedImportPlacement);
                revitLinkInstance.Pinned = props.PinLink;
                t.Commit();
            }
            catch (InvalidOperationException)
            {
                revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);
                ModelHelper.YesNoTaskDialog(DIFF_COORD, () => doc.AcquireCoordinates(revitLinkInstance.Id));
                revitLinkInstance.Pinned = props.PinLink;
                t.Commit();
            }
            catch
            {
                t.RollBack();
            }
        }

        private static WorksetConfiguration CloseWorksetsWithLinks(ModelPath modelPath, string[] prefixes)
        {
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.CloseAllWorksets);
            if (prefixes.Length == 0) return worksetConfiguration;

            //problem occurs if centralModel can't be found
            IList<WorksetId> worksetIds = WorksharingUtils.GetUserWorksetInfo(modelPath)
                .Where(wp => !prefixes.Any(wp.Name.StartsWith))
                .Select(wp => wp.Id)
                .ToList();

            worksetConfiguration.Open(worksetIds);
            return worksetConfiguration;
        }
    }
}