using System.Windows.Controls;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Extensions;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Brushes = System.Windows.Media.Brushes;

namespace AlterTools.BatchExport.Views.Params;

public static class ParamsHelper
{
    private static string _fileName;
    private static ParamsViewModel _paramsVm;
    public static void ExportParameters(this ListBoxItem item,
        Application app,
        ParamsViewModel paramsVm,
        CsvHelper csvHelper)
    {
        _paramsVm = paramsVm;
        
        string filePath = item.Content?.ToString();
        _fileName = Path.GetFileName(filePath);

        if (!File.Exists(filePath))
        {
            item.Background = Brushes.Red;
            return;
        }

        item.Background = Brushes.Blue;

        try
        {
            using Document doc = app.OpenDocument(filePath, out _);
            if (doc is null) return;

            using ElementCategoryFilter filterOutHvac = new(BuiltInCategory.OST_HVAC_Zones, true);

            IEnumerable<ParametersTable> paramTables = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(filterOutHvac)
                .Where(el => el.IsPhysicalElement())
                .Where(el => !string.IsNullOrWhiteSpace(
                    el.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM)
                        .GetValueString()))
                .Select(GetParametersTable);

            foreach (ParametersTable table in paramTables)
            {
                csvHelper.WriteElement(table);
            }
            
            doc.Close(false);
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

    private static ParametersTable GetParametersTable(this Element el)
    {
        return new ParametersTable
        {
            ModelName = _fileName,
            Parameters = el.GetParametersSet(_paramsVm.ParametersNames),
#if R24_OR_GREATER
            ElementId = el.Id.Value,
#else
            ElementId = el.Id.IntegerValue,
#endif
        };
    }
}