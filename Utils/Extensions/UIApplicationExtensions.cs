using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.Utils.Extensions;

public static class UIApplicationExtensions
{
    public static Workset[] GetWorksets(this UIApplication uiApp)
    {
        return
        [
            .. new FilteredWorksetCollector(uiApp.ActiveUIDocument.Document)
                .OfKind(WorksetKind.UserWorkset)
                .ToWorksets()
        ];
    }
}