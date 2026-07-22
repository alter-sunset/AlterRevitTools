using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.UI;

namespace AlterTools.atExportModel;

public class ExternalEventHandler : RevitEventWrapper<IConfigExport>
{
    public override void Execute(UIApplication uiApp, IConfigExport args)
    {
        throw new NotImplementedException();

        // 1. Check if file should be opened at all
        // 2. Check for nwc/ifc export
        // ?. Check for the need of report
        // 3. Check for the need of cleaning
    }
}