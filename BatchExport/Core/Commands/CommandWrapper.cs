using System.Windows;
using AlterTools.BatchExport.Utils;

namespace AlterTools.BatchExport.Core.Commands;

internal static class CommandWrapper
{
    internal static Result Execute(ref string msg, Func<Window> window)
    {
        try
        {
            ViewHelper.ShowForm(window);
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return Result.Failed;
        }
    }
}