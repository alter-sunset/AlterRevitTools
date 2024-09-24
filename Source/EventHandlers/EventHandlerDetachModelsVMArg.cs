using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.ApplicationServices;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Source.EventHandlers
{
    public class EventHandlerDetachModelsVMArg : EventHandlerBaseVMArgs
    {
        public override void Execute(UIApplication uiApp, ViewModelBase viewModelBase)
        {
            DetachViewModel detachViewModel = viewModelBase as DetachViewModel;
            if (!detachViewModel.IsEverythingFilled())
            {
                return;
            }

            List<ListBoxItem> listItems = [.. detachViewModel.ListBoxItems];

            using Application application = uiApp.Application;
            using ErrorSwallower errorSwallower = new(uiApp, application);

            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();
                if (!File.Exists(filePath))
                {
                    item.Background = Brushes.Red;
                    continue;
                }
                detachViewModel.DetachModel(application, filePath);
            }
            detachViewModel.Finisher("DetachModelsFinished");
        }
    }
}