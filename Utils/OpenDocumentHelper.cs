using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace VLS.BatchExportNet.Utils
{
    static class OpenDocumentHelper
    {
        internal static Document OpenAsIs(Application application, ModelPath modelPath, WorksetConfiguration worksetConfiguration)
        {
            OpenOptions openOptions = new()
            {
                DetachFromCentralOption = DetachFromCentralOption.DoNotDetach
            };
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            Document openedDoc = application.OpenDocumentFile(modelPath, openOptions);
            return openedDoc;
        }
        internal static Document OpenDetached(Application application, ModelPath modelPath, WorksetConfiguration worksetConfiguration)
        {
            OpenOptions openOptions = new()
            {
                DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets
            };
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            Document openedDoc = application.OpenDocumentFile(modelPath, openOptions);
            return openedDoc;
        }
        internal static Document OpenTransmitted(Application application, ModelPath modelPath)
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