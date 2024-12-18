using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using AlterTools.BatchExportNet.Utils;
using AlterTools.BatchExportNet.Views.Detach;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Source.EventHandlers
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
                using ErrorSwallower errorSwallower = new(uiApp);
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