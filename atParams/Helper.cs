using System.IO;
using AlterTools.Utils;
using AlterTools.Utils.Extensions;
using AlterTools.Utils.Logger;
using Autodesk.Revit.DB;
using Application = Autodesk.Revit.ApplicationServices.Application;
using View = Autodesk.Revit.DB.View;

namespace AlterTools.atParams;

public class Helper(string file, Application app, IConfigParams config, CsvHelper csvHelper, ILogger logger)
{
    private string FileName => Path.GetFileName(file);
    private readonly string[] _parametersNames = config.ParametersNames;
    private readonly string _viewName = config.ViewName;

    public void ExportParameters()
    {
        DateTime startTime = DateTime.Now;
        logger.Start(FileName);
        if (!File.Exists(file))
        {
            logger.Error($"File {file} not found.");
            return;
        }


        try
        {
            using Document doc = app.OpenDocument(file, out _);
            if (doc is null)
            {
                logger.Error($"Can't open document {FileName}.");
                return;
            }

            logger.FileOpened();

            View targetView = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .FirstOrDefault(v =>
                    !v.IsTemplate &&
                    v.Name.Equals(_viewName, StringComparison.OrdinalIgnoreCase));
            if (targetView is null)
            {
                logger.Error($"Can't find view {_viewName}.");
                return;
            }

            using ElementCategoryFilter filterOutHvac = new(BuiltInCategory.OST_HVAC_Zones, true);

            IEnumerable<ParametersTable> paramTables = new FilteredElementCollector(doc, targetView.Id)
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

            csvHelper.Flush();
            doc.Close(false);
            logger.Success("Export finished.");
            logger.TimeForFile(startTime);
        }
        catch
        {
            logger.Error($"Some kind of error on file {FileName}.");
            // ignored
        }

        logger.LineBreak();
    }

    private Dictionary<string, string> GetParametersSet(Element element, string[] parametersNames)
    {
        return parametersNames.ToDictionary(name => name, name => GetParameterString(element, name));
    }

    private static Parameter FindParameter(Element element, string parameterName)
    {
        Parameter param = element.LookupParameter(parameterName);

        if (param is not null && param.HasValue) return param;

        ElementId typeId = element.GetTypeId();

        if (typeId == ElementId.InvalidElementId) return null;

        Element type = element.Document.GetElement(typeId);

        param = type?.LookupParameter(parameterName);

        return param is not null && param.HasValue
            ? param
            : null;
    }

    private static string GetParameterString(Element element, string parameterName)
    {
        Parameter param = FindParameter(element, parameterName);

        return param == null
            ? string.Empty
            : param.GetValueString();
    }

    private ParametersTable GetParametersTable(Element el)
    {
        return new ParametersTable
        {
            ModelName = FileName,
            Parameters = GetParametersSet(el, _parametersNames),
#if R24_OR_GREATER
            ElementId = el.Id.Value,
#else
            ElementId = el.Id.IntegerValue,
#endif
        };
    }
}