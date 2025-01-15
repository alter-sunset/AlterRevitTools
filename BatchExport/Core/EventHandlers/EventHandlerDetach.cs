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
        public override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
        {
            if (iConfigBase is not DetachViewModel detachVM || !detachVM.IsEverythingFilled()) return;

            List<ListBoxItem> listItems = detachVM.ListBoxItems.ToList();

            using Application app = uiApp.Application;
            foreach (ListBoxItem item in listItems)
            {
                using ErrorSwallower errorSwallower = new ErrorSwallower(uiApp);
                string filePath = item.Content?.ToString();
                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }
                detachVM.DetachModel(app, filePath);
            }
            detachVM.Finisher(id: "DetachModelsFinished");
        }
    }
}