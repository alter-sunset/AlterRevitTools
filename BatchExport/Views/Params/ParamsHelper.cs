using AlterTools.BatchExport.Utils;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace AlterTools.BatchExport.Views.Params
{
    public static class ParamsHelper
    {
        public static void ExportParameters(this ListBoxItem item, Application app, ParamsViewModel paramsVm, CsvHelper csvHelper)
        {
            string filePath = item.Content?.ToString();
            string fileName = Path.GetFileName(filePath);

            if (!File.Exists(filePath))
            {
                item.Background = Brushes.Red;
                return;
            }

            item.Background = Brushes.Blue;

            try
            {
                using Document doc = OpenDocumentHelper.OpenDocument(app, filePath, out bool _);
                if (null == doc) return;

                IEnumerable<ParametersTable> paramTables = new FilteredElementCollector(doc)
                                                               .WhereElementIsNotElementType()
                                                               .Where(el => el.IsPhysicalElement())
                                                               .Select(el => new ParametersTable
                                                               {
                                                                   ModelName = fileName,
                                                                   Parameters = el.GetParametersSet(paramsVm.ParametersNames),

#if R24_OR_GREATER
                                                                   ElementId = el.Id.Value,
#else
                                                                   ElementId = el.Id.IntegerValue,
#endif
                                                               });

                foreach (ParametersTable table in paramTables)
                {
                    csvHelper.WriteElement(table);
                }
            }
            catch
            {
                // ignored
            }

            item.Background = Brushes.Green;
        }

        private static Dictionary<string, string> GetParametersSet(this Element element, string[] parametersNames)
        {
            return parametersNames.ToDictionary(name => name, name => element.LookupParameter(name).GetValueString());
        }
    }
}