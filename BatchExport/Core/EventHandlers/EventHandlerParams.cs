using System.Windows.Controls;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Params;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerParams : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not ParamsViewModel paramsVm) return;
        if (!paramsVm.IsEverythingFilled()) return;

        using (CsvHelper csvHelper = new(paramsVm.CsvPath, paramsVm.ParametersNames))
        {
            using ErrorSuppressor errorSuppressor = new(uiApp);
            using Application app = uiApp.Application;

            List<ListBoxItem> listItems = [.. paramsVm.ListBoxItems];
            listItems.ForEach(item => item.ExportParameters(app, paramsVm, csvHelper));
        }

        paramsVm.FinishWork("ExportParametersFinished");
    }
}