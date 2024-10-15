using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace VLS.BatchExportNet.Utils
{
    public static class OpenDocumentHelper
    {
        public static Document OpenAsIs(this ModelPath modelPath,
            Application application,
            WorksetConfiguration worksetConfiguration)
        {
            OpenOptions openOptions = CreateOpenOptions(DetachFromCentralOption.DoNotDetach);
            return modelPath.OpenDocument(openOptions, worksetConfiguration, application);
        }

        public static Document OpenDetached(this ModelPath modelPath,
            Application application,
            WorksetConfiguration worksetConfiguration)
        {
            OpenOptions openOptions = CreateOpenOptions(DetachFromCentralOption.DetachAndPreserveWorksets);
            return modelPath.OpenDocument(openOptions, worksetConfiguration, application);
        }

        public static Document OpenTransmitted(this ModelPath modelPath, Application application)
        {
            OpenOptions openOptions = CreateOpenOptions(DetachFromCentralOption.ClearTransmittedSaveAsNewCentral);
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.CloseAllWorksets);
            return modelPath.OpenDocument(openOptions, worksetConfiguration, application);
        }

        private static OpenOptions CreateOpenOptions(DetachFromCentralOption detachOption) =>
            new() { DetachFromCentralOption = detachOption };

        private static Document OpenDocument(this ModelPath modelPath,
            OpenOptions openOptions,
            WorksetConfiguration worksetConfiguration,
            Application application)
        {
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            return application.OpenDocumentFile(modelPath, openOptions);
        }
    }
}