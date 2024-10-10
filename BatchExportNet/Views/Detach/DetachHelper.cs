using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.IO;
using System.Linq;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Views.Detach
{
    public static class DetachHelper
    {
        public static void DetachModel(this IConfigDetach iConfigDetach, Application application, string filePath)
        {
            try
            {
                (Document document, bool isWorkshared) = OpenDocument(application, filePath);
                if (document == null) return;

                ProcessDocument(document, iConfigDetach);
                string fileDetachedPath = GetDetachedFilePath(iConfigDetach, document, filePath);
                SaveDocument(document, fileDetachedPath, isWorkshared);
                Cleanup(document, fileDetachedPath, isWorkshared);
            }
            catch { }
        }
        private static (Document, bool) OpenDocument(Application application, string filePath)
        {
            Document document = null;
            bool isWorkshared = false;

            try
            {
                BasicFileInfo fileInfo = BasicFileInfo.Extract(filePath);
                if (!fileInfo.IsWorkshared)
                {
                    document = application.OpenDocumentFile(filePath);
                }
                else
                {
                    ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                    WorksetConfiguration worksetConfig = new(WorksetConfigurationOption.CloseAllWorksets);
                    document = modelPath.OpenDetached(application, worksetConfig);
                    isWorkshared = true;
                }
            }
            catch { }

            return (document, isWorkshared);
        }
        private static void ProcessDocument(Document document, IConfigDetach iConfigDetach)
        {
            document.DeleteAllLinks();

            if (iConfigDetach.RemoveEmptyWorksets && document.IsWorkshared)
                document.RemoveEmptyWorksets();

            if (iConfigDetach.Purge)
                document.PurgeAll();
        }
        private static string GetDetachedFilePath(IConfigDetach iConfigDetach, Document document, string originalFilePath)
        {
            string documentTitle = document.Title
                .Replace("_detached", "")
                .Replace("_отсоединено", "");

            string fileDetachedPath = Path.Combine(iConfigDetach.FolderPath, $"{documentTitle}.rvt");

            if (iConfigDetach is DetachViewModel detachViewModel && detachViewModel.RadioButtonMode == 2)
                fileDetachedPath = RenamePath(originalFilePath,
                    RenameType.Folder,
                    detachViewModel.MaskIn, detachViewModel.MaskOut);

            if (iConfigDetach.IsToRename)
                fileDetachedPath = RenamePath(fileDetachedPath,
                    RenameType.Title,
                    iConfigDetach.MaskInName, iConfigDetach.MaskOutName);

            if (iConfigDetach.CheckForEmptyView)
                CheckAndModifyForEmptyView(document, iConfigDetach, ref fileDetachedPath);

            return fileDetachedPath;
        }

        private static void CheckAndModifyForEmptyView(Document document, IConfigDetach iConfigDetach, ref string fileDetachedPath)
        {
            document.OpenAllWorksets();
            try
            {
                string onlyTitle = Path.GetFileNameWithoutExtension(fileDetachedPath);
                string folder = Path.GetDirectoryName(fileDetachedPath);
                string extension = Path.GetExtension(fileDetachedPath);
                Element view = new FilteredElementCollector(document)
                    .OfClass(typeof(View3D))
                    .FirstOrDefault(e => e.Name == iConfigDetach.ViewName && !((View3D)e).IsTemplate);

                if (view is not null && document.IsViewEmpty(view))
                    fileDetachedPath = RenamePath(fileDetachedPath, RenameType.Empty);
            }
            catch { }
        }
        private static void SaveDocument(Document document, string fileDetachedPath, bool isWorkshared)
        {
            SaveAsOptions saveOptions = new()
            {
                OverwriteExistingFile = true,
                MaximumBackups = 1
            };

            if (isWorkshared)
            {
                WorksharingSaveAsOptions worksharingOptions = new() { SaveAsCentral = true };
                saveOptions.SetWorksharingOptions(worksharingOptions);
            }

            try
            {
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
                document.SaveAs(modelPath, saveOptions);
            }
            catch { }
        }
        private static void Cleanup(Document document, string fileDetachedPath, bool isWorkshared)
        {
            try
            {
                if (isWorkshared)
                {
                    document.FreeTheModel();
                    ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
                    UpdateTransmissionData(modelPath);
                    Directory.Delete(fileDetachedPath.Replace(".rvt", "_backup"), true);
                }
                else
                {
                    File.Delete(fileDetachedPath.Replace(".rvt", ".0001.rvt"));
                }
            }
            catch { }
            finally
            {
                document?.Close();
            }
        }
        private static void UpdateTransmissionData(ModelPath modelPath)
        {
            TransmissionData transmissionData = TransmissionData.ReadTransmissionData(modelPath);
            if (transmissionData is not null)
            {
                transmissionData.IsTransmitted = true;
                TransmissionData.WriteTransmissionData(modelPath, transmissionData);
            }
        }
        private static string RenamePath(string filePath, RenameType renameType, string maskIn = "", string maskOut = "")
        {
            string title = Path.GetFileNameWithoutExtension(filePath);
            string folder = Path.GetDirectoryName(filePath);
            string extension = Path.GetExtension(filePath);

            switch (renameType)
            {
                case RenameType.Folder:
                    folder = folder.Replace(maskIn, maskOut);
                    break;
                case RenameType.Title:
                    title = title.Replace(maskIn, maskOut);
                    break;
                case RenameType.Empty:
                    title = $"EMPTY_{title}";
                    break;
            }

            return Path.Combine(folder, $"{title}{extension}");
        }
    }

    public enum RenameType
    {
        Folder,
        Title,
        Empty
    }
}