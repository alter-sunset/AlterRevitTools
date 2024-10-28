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
            List<Entry> listItems = [.. linkViewModel.Entries];

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
            //check for share coordinates
            //if (doc.ActiveView)

            foreach (Entry item in listItems)
            {
                string filePath = item.Name;

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
                    RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, item.SelectedOptionalValue);
                    t.Commit();
                }
                catch (InvalidOperationException ex)
                {
                    string title = "Ошибка";
                    string message = "Обнаружено различие систем координат. Выполнить получение коордианат из файла?";

                    TaskDialogResult result = TaskDialog.Show(title, message, TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);

                    if (result is not TaskDialogResult.Yes)
                    {
                        t.RollBack();
                        continue;
                    }
                    RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);
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