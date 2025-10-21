using System.Windows;
using AlterTools.BatchExport.Utils;

namespace AlterTools.BatchExport.Core.Commands;

public abstract class ExternalCommandBase : IExternalCommand
{
    protected virtual Func<Window> WindowFactory => null;
    internal static ExternalCommandData CommandData;
    
    public Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
    {
        CommandData =  commandData;
        
        try
        {
            ViewHelper.ShowForm(WindowFactory);
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return Result.Failed;
        }
    }
}