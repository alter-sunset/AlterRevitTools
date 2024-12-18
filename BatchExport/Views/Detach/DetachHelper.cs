using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using System.IO;
using System.Linq;
using VLS.BatchExport.Utils;

namespace VLS.BatchExport.Views.Detach
{
    public static class DetachHelper
    {
        public static void DetachModel(this IConfigDetach iConfigDetach, Application app, string filePath)
        {
            try
            {
                (Document doc, bool isWorkshared) = OpenDocument(app, filePath);
                if (doc is null) return;

                ProcessDocument(doc, iConfigDetach);
                string fileDetachedPath = GetDetachedFilePath(iConfigDetach, doc, filePath);
                SaveDocument(doc, fileDetachedPath, isWorkshared);
                Cleanup(doc, fileDetachedPath, isWorkshared);
            }
            catch { }
        }
        private static (Document, bool) OpenDocument(Application app, string filePath)
        {
            Document doc = null;
            bool isWorkshared = false;

            try
            {
                BasicFileInfo fileInfo = BasicFileInfo.Extract(filePath);
                if (!fileInfo.IsWorkshared)
                {
                    doc = app.OpenDocumentFile(filePath);
                }
                else
                {
                    ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                    WorksetConfiguration worksetConfig = new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets);
                    doc = modelPath.OpenDetached(app, worksetConfig);
                    isWorkshared = true;
                }
            }
            catch { }

            return (doc, isWorkshared);
        }
        private static void ProcessDocument(Document doc, IConfigDetach iConfigDetach)
        {
            doc.DeleteAllLinks();
#if R23 || R24
            if (iConfigDetach.RemoveEmptyWorksets && doc.IsWorkshared)
                doc.RemoveEmptyWorksets();
#endif
#if R24
            if (iConfigDetach.Purge)
                doc.PurgeAll();
#endif
        }
        private static string GetDetachedFilePath(IConfigDetach iConfigDetach, Document doc, string originalFilePath)
        {
            string docTitle = doc.Title.RemoveDetach();

            string fileDetachedPath = Path.Combine(iConfigDetach.FolderPath, $"{docTitle}.rvt");

            if (iConfigDetach is DetachViewModel detachViewModel && detachViewModel.RadioButtonMode == 2)
                fileDetachedPath = RenamePath(originalFilePath,
                    RenameType.Folder,
                    detachViewModel.MaskIn, detachViewModel.MaskOut);

            if (iConfigDetach.IsToRename)
                fileDetachedPath = RenamePath(fileDetachedPath,
                    RenameType.Title,
                    iConfigDetach.MaskInName, iConfigDetach.MaskOutName);

            if (iConfigDetach.CheckForEmptyView)
                CheckAndModifyForEmptyView(doc, iConfigDetach, ref fileDetachedPath);

            return fileDetachedPath;
        }

        private static void CheckAndModifyForEmptyView(Document doc, IConfigDetach iConfigDetach, ref string fileDetachedPath)
        {
            doc.OpenAllWorksets();
            try
            {
                string onlyTitle = Path.GetFileNameWithoutExtension(fileDetachedPath);
                string folder = Path.GetDirectoryName(fileDetachedPath);
                string extension = Path.GetExtension(fileDetachedPath);
                Element view = new FilteredElementCollector(doc)
                    .OfClass(typeof(View3D))
                    .FirstOrDefault(e => e.Name == iConfigDetach.ViewName && !((View3D)e).IsTemplate);

                if (!(view is null) && doc.IsViewEmpty(view))
                    fileDetachedPath = RenamePath(fileDetachedPath, RenameType.Empty);
            }
            catch { }
        }
        private static void SaveDocument(Document doc, string fileDetachedPath, bool isWorkshared)
        {
            SaveAsOptions saveOptions = new SaveAsOptions()
            {
                OverwriteExistingFile = true,
                MaximumBackups = 1
            };

            if (isWorkshared)
            {
                WorksharingSaveAsOptions worksharingOptions = new WorksharingSaveAsOptions() { SaveAsCentral = true };
                saveOptions.SetWorksharingOptions(worksharingOptions);
            }

            try
            {
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
                doc.SaveAs(modelPath, saveOptions);
            }
            catch { }
        }
        private static void Cleanup(Document doc, string fileDetachedPath, bool isWorkshared)
        {
            try { doc.FreeTheModel(); }
            catch { }
            finally { doc?.Close(); }

            if (isWorkshared)
            {
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
                UpdateTransmissionData(modelPath);
                string backupFolderPath = fileDetachedPath.Replace(".rvt", "_backup");
                if (Directory.Exists(backupFolderPath))
                    Directory.Delete(backupFolderPath, true);
                return;
            }
            for (int i = 1; i <= 3; i++)
            {
                string versionedFilePath = fileDetachedPath.Replace(".rvt", $".{i:D4}.rvt");
                if (File.Exists(versionedFilePath))
                    File.Delete(versionedFilePath);
            }
        }
        private static void UpdateTransmissionData(ModelPath modelPath)
        {
            TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
            if (transData is null) return;
            transData.IsTransmitted = true;
            TransmissionData.WriteTransmissionData(modelPath, transData);
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