using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerDetach : EventHandlerBase
    {
        public override void Execute(UIApplication uiApp, IConfigBase viewModelBase)
        {
            if (viewModelBase is not DetachViewModel detachViewModel || !detachViewModel.IsEverythingFilled()) return;

            List<ListBoxItem> listItems = detachViewModel.ListBoxItems.ToList();

            using Application app = uiApp.Application;

            foreach (ListBoxItem item in listItems)
            {
                using ErrorSwallower errorSwallower = new(uiApp, app);
                string filePath = item.Content?.ToString();
                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }
                detachViewModel.DetachModel(app, filePath);
            }
            detachViewModel.Finisher(id: "DetachModelsFinished");
        }
    }
}