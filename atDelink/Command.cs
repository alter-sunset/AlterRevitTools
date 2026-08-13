using AlterTools.Utils;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.atDelink;

[Transaction(TransactionMode.Manual)]
public class Command : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        using UIApplication uiApp = commandData.Application;
        using Document doc = uiApp.ActiveUIDocument.Document;
        using ErrorSuppressor errorSuppressor = new(uiApp);

        doc.DeleteAllLinks(false);

        return Result.Succeeded;
    }
}