using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.Exceptions;

namespace VLS.BatchExportNet.Views.Link
{
    internal static class LinkHelper
    {
        internal static void CreateLinks(this LinkViewModel linkViewModel, UIApplication uiApp)
        {
            using Application application = uiApp.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            bool isCurrentWorkset = linkViewModel.IsCurrentWorkset;
            List<Entry> entries = [.. linkViewModel.Entries];

            ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(doc.PathName);
            IList<WorksetPreview> worksets = null;
            WorksetTable worksetTable = null;
            if (!isCurrentWorkset)
            {
                try
                {
                    worksets = WorksharingUtils.GetUserWorksetInfo(modelPath);
                    worksetTable = doc.GetWorksetTable();
                }
                catch
                {
                    isCurrentWorkset = false;
                }
            }
            RevitLinkOptions options = new(false);

            foreach (Entry entry in entries)
            {
                string filePath = entry.Name;

                if (!File.Exists(filePath)) continue;

                using Transaction t = new(doc);
                t.Start($"Link {filePath}");

                if (!isCurrentWorkset && worksets is not null && worksetTable is not null)
                {
                    WorksetId worksetId = worksets.FirstOrDefault(e => filePath.Contains(e.Name.Split('_')[0]))?.Id;
                    worksetTable.SetActiveWorksetId(worksetId);
                }
                LinkLoadResult linkLoadResult = null;
                ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                try
                {
                    linkLoadResult = RevitLinkType.Create(doc, linkPath, options);
                    RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, entry.SelectedImportPlacement);
                    t.Commit();
                }
                catch (InvalidOperationException ex)
                {
                    RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);

                    string title = "Ошибка";
                    string message = "Обнаружено различие систем координат. Выполнить получение коордианат из файла?";
                    TaskDialogResult result = TaskDialog.Show(title, message, TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
                    if (result is TaskDialogResult.Yes)
                        doc.AcquireCoordinates(revitLinkInstance.Id);

                    t.Commit();
                }
                catch
                {
                    t.RollBack();
                }
            }
        }
    }
}