using System.IO;
using AlterTools.Utils;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atParams;

public static class Helper
{
    private static string _fileName;
    private static string[] _parametersNames;

    public static void ExportParameters(string file, Application app, string[] parametersNames, CsvHelper csvHelper)
    {
        _fileName = Path.GetFileName(file);
        _parametersNames = parametersNames;

        if (!File.Exists(file)) return;

        try
        {
            using Document doc = app.OpenDocument(file, out _);
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
    }

    private static Dictionary<string, string> GetParametersSet(this Element element, string[] parametersNames)
    {
        return parametersNames.ToDictionary(name => name, element.GetParameterString);
    }

    private static string GetParameterString(this Element element, string parameterName)
    {
        using Parameter param = element.LookupParameter(parameterName);
        try
        {
            return param.HasValue ? param.GetValueString() : string.Empty;
        }
        catch
        {
            return param.GetValueString();
        }
    }

    private static ParametersTable GetParametersTable(this Element el)
    {
        return new ParametersTable
        {
            ModelName = _fileName,
            Parameters = el.GetParametersSet(_parametersNames),
#if R24_OR_GREATER
            ElementId = el.Id.Value,
#else
            ElementId = el.Id.IntegerValue,
#endif
        };
    }
}