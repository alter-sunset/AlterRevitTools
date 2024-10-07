using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.IO;
using System.Linq;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.Detach
{
    public static class DetachHelper
    {
        public static void DetachModel(this IConfigDetach iConfigDetach, Application application, string filePath)
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

            if (iConfigDetach.RemoveEmptyWorksets && isWorkshared)
                document.RemoveEmptyWorksets();

            if (iConfigDetach.Purge)
                document.PurgeAll();

            string documentTitle = document.Title.Replace("_detached", "").Replace("_отсоединено", "");
            if (iConfigDetach.IsToRename)
                documentTitle = documentTitle.Replace(iConfigDetach.MaskInName, iConfigDetach.MaskOutName);

            string fileDetachedPath = "";

            if (iConfigDetach is DetachViewModel detachViewModel)
            {
                switch (detachViewModel.RadioButtonMode)
                {
                    case 1:
                        string folder = detachViewModel.FolderPath;
                        string titleWithExtension = documentTitle + ".rvt";
                        fileDetachedPath = Path.Combine(folder, titleWithExtension);
                        break;
                    case 2:
                        string maskIn = detachViewModel.MaskIn;
                        string maskOut = detachViewModel.MaskOut;
                        fileDetachedPath = @filePath.Replace(maskIn, maskOut)
                            .Replace(detachViewModel.MaskInName, detachViewModel.MaskOutName);
                        break;
                }
            }

            if (iConfigDetach.CheckForEmptyView)
            {
                document.OpenAllWorksets();
                using FilteredElementCollector stuff = new(document);
                try
                {
                    string titleEmpty = Path.GetFileNameWithoutExtension(fileDetachedPath);

                    Element view = stuff.OfClass(typeof(View3D))
                        .FirstOrDefault(e => e.Name == iConfigDetach.ViewName && !((View3D)e).IsTemplate);

                    if (view is not null
                        && document.IsViewEmpty(view))
                        fileDetachedPath = fileDetachedPath.Replace(titleEmpty, $"EMPTY_{titleEmpty}");
                }
                catch { }
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

            if (isWorkshared)
            {
                try
                {
                    document.FreeTheModel();
                }
                catch { }
            }

            document?.Close();

            if (isWorkshared)
            {
                TransmissionData transmissionData = TransmissionData.ReadTransmissionData(modelDetachedPath);
                if (transmissionData is not null)
                {
                    transmissionData.IsTransmitted = true;
                    TransmissionData.WriteTransmissionData(modelDetachedPath, transmissionData);
                }
                try
                {
                    Directory.Delete(fileDetachedPath.Replace(".rvt", "_backup"), true);
                }
                catch { }
                return;
            }

            try
            {
                File.Delete(fileDetachedPath.Replace(".rvt", ".0001.rvt"));
            }
            catch { }
            return;
        }
    }
}