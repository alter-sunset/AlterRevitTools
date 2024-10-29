using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Exceptions;
using System.IO;
using System.Linq;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.Link
{
    internal static class LinkHelper
    {
        private const string DIFF_COORD = "Обнаружено различие систем координат. Выполнить получение коордианат из файла?";
        internal static void CreateLinks(this LinkViewModel linkViewModel, UIApplication uiApp)
        {
            Document doc = uiApp.ActiveUIDocument.Document;
            bool isCurrentWorkset = linkViewModel.IsCurrentWorkset;
            Entry[] entries = linkViewModel.Entries
                .OrderBy(e => e.SelectedWorkset.Name)
                .ToArray();

            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(doc.PathName);
            WorksetTable worksetTable = !isCurrentWorkset ? GetWorksetTable(doc, ref isCurrentWorkset) : null;

            RevitLinkOptions options = new(false);
            bool setWorksetId = !isCurrentWorkset && linkViewModel.Worksets.Length > 0;

            foreach (Entry entry in entries)
            {
                if (!File.Exists(entry.Name)) continue;

                using Transaction t = new(doc);
                t.Start($"Link {entry.Name}");

                if (setWorksetId)
                    worksetTable.SetActiveWorksetId(entry.SelectedWorkset.Id);

                if (!TryCreateLink(doc, entry, options, t))
                    t.RollBack();
            }
        }
        private static WorksetTable GetWorksetTable(Document doc, ref bool isCurrentWorkset)
        {
            try
            {
                return doc.GetWorksetTable();
            }
            catch
            {
                isCurrentWorkset = false;
                return null;
            }
        }
        private static bool TryCreateLink(Document doc, Entry entry, RevitLinkOptions options, Transaction t)
        {
            RevitLinkInstance revitLinkInstance;
            LinkLoadResult linkLoadResult = default;
            ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(entry.Name);

            try
            {
                linkLoadResult = RevitLinkType.Create(doc, linkPath, options);
                _ = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, entry.SelectedImportPlacement);
                t.Commit();
                return true;
            }
            catch (InvalidOperationException)
            {
                revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);
                ModelHelper.YesNoTaskDialog(DIFF_COORD, () => doc.AcquireCoordinates(revitLinkInstance.Id));
                t.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}