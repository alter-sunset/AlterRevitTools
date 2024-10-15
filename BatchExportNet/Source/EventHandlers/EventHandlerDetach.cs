using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.IO;
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
            DetachViewModel detachViewModel = viewModelBase as DetachViewModel;
            if (!detachViewModel.IsEverythingFilled()) return;

            List<ListBoxItem> listItems = [.. detachViewModel.ListBoxItems];

            using Application application = uiApp.Application;

            foreach (ListBoxItem item in listItems)
            {
                using ErrorSwallower errorSwallower = new(uiApp, application);
                string filePath = item.Content.ToString();
                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }
                detachViewModel.DetachModel(application, filePath);
            }
            detachViewModel.Finisher(id: "DetachModelsFinished");
        }
    }
}