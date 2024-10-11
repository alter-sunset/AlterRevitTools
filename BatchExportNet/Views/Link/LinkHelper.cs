using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

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
            List<ListBoxItem> listItems = [.. linkViewModel.ListBoxItems];

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

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath)) continue;

                using Transaction t = new(doc);
                t.Start($"Link {filePath}");

                if (!isCurrentWorkset && worksets is not null && worksetTable is not null)
                {
                    WorksetId worksetId = worksets.FirstOrDefault(e => filePath.Contains(e.Name.Split('_')[0]))?.Id;
                    worksetTable.SetActiveWorksetId(worksetId);
                }

                ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                try
                {
                    LinkLoadResult linkLoadResult = RevitLinkType.Create(doc, linkPath, options);
                    RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Shared);
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