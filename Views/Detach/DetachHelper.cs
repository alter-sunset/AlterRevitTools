using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.Detach
{
    internal static class DetachHelper
    {
        internal static void DetachModel(this DetachViewModel detachViewModel, Application application, string filePath)
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
            switch (detachViewModel.RadioButtonMode)
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
            catch { }

            document?.Close();
            document?.Dispose();
        }
    }
}