using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Exceptions;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.Link
{
    internal static class LinkHelper
    {
        public static RevitLinkOptions RevitLinkOptions => new(false);
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
            LinkProps props = new(doc.GetWorksetTable(), setWorksetId);

            entries.ForEach(entry => TryCreateLink(doc, entry, props));
        }
        private static void TryCreateLink(Document doc, Entry entry, LinkProps props)
        {
            using Transaction t = new(doc);
            t.Start($"Link {entry.Name}");

            if (props.SetWorksetId)
                props.WorksetTable.SetActiveWorksetId(entry.SelectedWorkset.Id);

            RevitLinkInstance revitLinkInstance;
            LinkLoadResult linkLoadResult = default;
            ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(entry.Name);

            try
            {
                linkLoadResult = RevitLinkType.Create(doc, linkPath, RevitLinkOptions);
                _ = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, entry.SelectedImportPlacement);
                t.Commit();
            }
            catch (InvalidOperationException)
            {
                revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);
                ModelHelper.YesNoTaskDialog(DIFF_COORD, () => doc.AcquireCoordinates(revitLinkInstance.Id));
                t.Commit();
            }
            catch
            {
                t.RollBack();
            }
        }
    }
}