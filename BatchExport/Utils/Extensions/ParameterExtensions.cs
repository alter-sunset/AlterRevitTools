namespace AlterTools.BatchExport.Utils.Extensions;

public static class ParameterExtensions
{
    /// <summary>
    ///     Return parameter value as string with correct null check
    /// </summary>
    public static string GetValueString(this Parameter param)
    {
        return param?.AsValueString() is null
            ? string.Empty
            : param.AsValueString().Trim();
    }
}