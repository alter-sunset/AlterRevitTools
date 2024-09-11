using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace VLS.BatchExportNet.Utils
{
    public static class OpenDocument
    {
        public static Document OpenAsIs(Application application, ModelPath modelPath, WorksetConfiguration worksetConfiguration)
        {
            OpenOptions openOptions = new()
            {
                DetachFromCentralOption = DetachFromCentralOption.DoNotDetach
            };
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            Document openedDoc = application.OpenDocumentFile(modelPath, openOptions);
            return openedDoc;
        }
        public static Document OpenDetached(Application application, ModelPath modelPath, WorksetConfiguration worksetConfiguration)
        {
            OpenOptions openOptions = new()
            {
                DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets
            };
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            Document openedDoc = application.OpenDocumentFile(modelPath, openOptions);
            return openedDoc;
        }
        public static Document OpenTransmitted(Application application, ModelPath modelPath)
        {
            OpenOptions openOptions = new()
            {
                DetachFromCentralOption = DetachFromCentralOption.ClearTransmittedSaveAsNewCentral
            };
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.CloseAllWorksets);
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            Document openedDoc = application.OpenDocumentFile(modelPath, openOptions);
            return openedDoc;
        }
    }
}