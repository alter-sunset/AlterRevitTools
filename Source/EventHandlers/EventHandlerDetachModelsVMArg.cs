using Autodesk.Revit.DB;
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
using VLS.BatchExportNet.Views;

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

            using Application application = uiApp.Application;
            List<ListBoxItem> listItems = [.. detachViewModel.ListBoxItems];

            uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
            application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);
            foreach (ListBoxItem item in listItems)
            {
                string filePath = item.Content.ToString();

                if (!File.Exists(filePath))
                {
                    string error = $"Файла {filePath} не существует. Ты совсем Туттуру?";
                    item.Background = Brushes.Red;
                    continue;
                }

                DetachModel(application, filePath, detachViewModel);
            }
            uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(ErrorSwallowersHelper.TaskDialogBoxShowingEvent);
            application.FailuresProcessing -= new EventHandler<FailuresProcessingEventArgs>(ErrorSwallowersHelper.Application_FailuresProcessing);

            detachViewModel.Finisher("DetachModelsFinished");
        }
        private static void DetachModel(Application application, string filePath, DetachViewModel detachViewModel)
        {
            Document document;
            BasicFileInfo fileInfo;
            bool isWorkshared;
            try
            {
                fileInfo = BasicFileInfo.Extract(filePath);
                if (!fileInfo.IsWorkshared)
                {
                    document = application.OpenDocumentFile(filePath);
                    isWorkshared = false;
                }
                else
                {
                    ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                    WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.CloseAllWorksets);
                    document = modelPath.OpenDetached(application, worksetConfiguration);
                    isWorkshared = true;
                }
            }
            catch
            {
                return;
            }
            document.DeleteAllLinks();
            string fileDetachedPath = "";
            switch (detachViewModel.RadionButtonMode)
            {
                case 1:
                    string folder = detachViewModel.FolderPath;
                    fileDetachedPath = folder + "\\" + document.Title.Replace("_detached", "").Replace("_отсоединено", "") + ".rvt";
                    break;
                case 2:
                    string maskIn = detachViewModel.MaskIn;
                    string maskOut = detachViewModel.MaskOut;
                    fileDetachedPath = @filePath.Replace(maskIn, maskOut);
                    break;
            }

            SaveAsOptions saveAsOptions = new()
            {
                OverwriteExistingFile = true,
                MaximumBackups = 1
            };
            WorksharingSaveAsOptions worksharingSaveAsOptions = new()
            {
                SaveAsCentral = true
            };
            if (isWorkshared)
                saveAsOptions.SetWorksharingOptions(worksharingSaveAsOptions);

            ModelPath modelDetachedPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
            document?.SaveAs(modelDetachedPath, saveAsOptions);

            try
            {
                if (isWorkshared)
                    document.FreeTheModel();
            }
            catch
            {
            }

            document?.Close();
            document?.Dispose();
        }
    }
}