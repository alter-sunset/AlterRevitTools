﻿using AlterTools.BatchExport.Utils;
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
        public static void ExportParameters(this ListBoxItem item, Application app, ParamsViewModel paramsVM, CsvHelper csvHelper)
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
                if (doc is null) return;

                IEnumerable<ParametersTable> paramTables = new FilteredElementCollector(doc)
                                                               .WhereElementIsNotElementType()
                                                               .Where(e => e.IsPhysicalElement())
                                                               .Select(e => new ParametersTable()
                                                               {
                                                                   ModelName = fileName,
                                                                   Parameters = e.GetParametersSet(paramsVM.ParametersNames),

#if R24_OR_GREATER
                                                                   ElementId = e.Id.Value,
#else
                                                                   ElementId = e.Id.IntegerValue,
#endif
                                                               });

                foreach (ParametersTable table in paramTables)
                {
                    csvHelper.WriteElement(table);
                }
            }
            catch { }

            item.Background = Brushes.Green;
        }

        private static Dictionary<string, string> GetParametersSet(this Element element, string[] parametersNames)
        {
            return parametersNames.ToDictionary(p => p, p => element.LookupParameter(p).GetValueString());
        }
    }
}