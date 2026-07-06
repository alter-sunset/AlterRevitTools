using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Utils.Extensions;
using Autodesk.Revit.Attributes;

namespace AlterTools.BatchExport.Core.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ExternalCommandDelink : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        using UIApplication uiApp = commandData.Application;
        using Document doc = uiApp.ActiveUIDocument.Document;
        using ErrorSuppressor errorSuppressor = new(uiApp);

        doc.DeleteAllLinks();

        return Result.Succeeded;
    }
}