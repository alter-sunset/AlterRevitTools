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
            if (iConfigBase is not ParamsViewModel paramsVM || !paramsVM.IsEverythingFilled()) return;

            List<ListBoxItem> listItems = paramsVM.ListBoxItems.ToList();
            using CsvHelper csvHelper = new(paramsVM.CsvPath, paramsVM.ParametersNames);
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
                item.Background = Brushes.Blue;
                try
                {
                    Document doc = OpenDocumentHelper.OpenDocument(app, filePath, out bool _);
                    if (doc is null) return;

                    IEnumerable<ParametersTable> elements = new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .Where(e => e.IsPhysicalElement())
                        .Select(e => new ParametersTable()
                        {
                            ModelName = fileName,
#if R24_OR_GREATER
                            ElementId = e.Id.Value,
#else
                            ElementId = e.Id.IntegerValue,
#endif
                            Parameters = GetParametersSet(e, paramsVM.ParametersNames)
                        });

                    foreach (ParametersTable table in elements)
                    {
                        csvHelper.WriteElement(table);
                    }
                }
                catch { }
                item.Background = Brushes.Green;
            }
            csvHelper.Dispose();
            paramsVM.Finisher(id: "ExportParametersFinished");
        }
        private static Dictionary<string, string> GetParametersSet(Element element, string[] parametersNames)
            => parametersNames.ToDictionary(p => p, p => element.LookupParameter(p).GetValueString());
    }
}