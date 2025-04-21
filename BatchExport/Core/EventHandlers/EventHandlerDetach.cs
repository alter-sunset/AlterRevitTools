using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Detach;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.EventHandlers
{
    public class EventHandlerDetach : EventHandlerBase
    {
        protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not DetachViewModel detachVm)
            {
                return;
            }

            if (!detachVm.IsEverythingFilled())
            {
                return;
            }

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

            detachVm.Finisher("DetachModelsFinished");
        }
    }
}