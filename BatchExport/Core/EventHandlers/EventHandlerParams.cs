using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Params;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerParams : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not ParamsViewModel paramsVM) return;
            if (!paramsVM.IsEverythingFilled()) return;

            using (CsvHelper csvHelper = new(paramsVM.CsvPath, paramsVM.ParametersNames))
            {
                using ErrorSuppressor errorSuppressor = new(uiApp);
                using Application app = uiApp.Application;

                List<ListBoxItem> listItems = paramsVM.ListBoxItems.ToList();
                listItems.ForEach(e => e.ExportParameters(app, paramsVM, csvHelper));
            }

            paramsVM.Finisher(id: "ExportParametersFinished");
        }
    }
}