using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Params;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;

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

            List<ListBoxItem> listItems = paramsVm.ListBoxItems.ToList();
            listItems.ForEach(item => item.ExportParameters(app, paramsVm, csvHelper));
        }

        paramsVm.Finisher("ExportParametersFinished");
    }
}