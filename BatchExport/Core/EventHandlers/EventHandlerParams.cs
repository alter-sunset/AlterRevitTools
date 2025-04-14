using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Params;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerParams : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not ParamsViewModel paramsVM) return;

            List<ListBoxItem> listItems = paramsVM.ListBoxItems.ToList();
            using CsvHelper csvHelper = new(paramsVM.CsvPath, paramsVM.ParametersNames); // add csvPath field in view
            using Application app = uiApp.Application;
            foreach (ListBoxItem item in listItems)
            {
                using ErrorSwallower errorSwallower = new(uiApp);
                string filePath = item.Content?.ToString();
                string fileName = Path.GetFileName(filePath);
                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }
                try
                {
                    Document doc = OpenDocument(app, filePath);
                    if (doc is null) return;

                    IEnumerable<ParametersTable> elements = new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .Select(e => new ParametersTable()
                        {
                            ModelName = fileName,
                            ElementId = e.Id.IntegerValue,
                            Parameters = GetParametersSet(e, paramsVM.ParametersNames)
                        });

                    foreach (ParametersTable table in elements)
                    {
                        csvHelper.WriteElement(table);
                    }
                }
                catch { }
            }
            csvHelper.Dispose();
            paramsVM.Finisher(id: "ExportParametersFinished");
        }
        private static Document OpenDocument(Application app, string filePath)
        {
            Document doc = null;

            try
            {
                BasicFileInfo fileInfo = BasicFileInfo.Extract(filePath);
                if (!fileInfo.IsWorkshared)
                {
                    doc = app.OpenDocumentFile(filePath);
                }
                else
                {
                    ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                    WorksetConfiguration worksetConfig = new(WorksetConfigurationOption.CloseAllWorksets);
                    doc = modelPath.OpenDetached(app, worksetConfig);
                }
            }
            catch { }

            return doc;
        }
        private static Dictionary<string, string> GetParametersSet(Element element, string[] parametersNames)
            => parametersNames.ToDictionary(e => e, e => element.LookupParameter(e).GetValueString());
    }
}