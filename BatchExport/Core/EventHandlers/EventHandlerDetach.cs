using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Detach;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerDetach : EventHandlerBase
    {
        protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not DetachViewModel detachVm) return;
            if (!detachVm.IsEverythingFilled()) return;

            using Application app = uiApp.Application;
            using ErrorSuppressor errorSuppressor = new(uiApp);

            List<ListBoxItem> listItems = detachVm.ListBoxItems.ToList();

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content?.ToString();

                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }

                detachVm.DetachModel(app, filePath);
            }

            detachVm.Finisher(id: "DetachModelsFinished");
        }
    }
}