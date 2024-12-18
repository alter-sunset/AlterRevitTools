using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace AlterTools.BatchExport.Utils
{
    public static class OpenDocumentHelper
    {
        public static Document OpenAsIs(this ModelPath modelPath, Application app, WorksetConfiguration worksetConfiguration) =>
            modelPath.OpenDocumentWithOptions(
                DetachFromCentralOption.DoNotDetach,
                worksetConfiguration,
                app);

        public static Document OpenDetached(this ModelPath modelPath, Application app, WorksetConfiguration worksetConfiguration) =>
            modelPath.OpenDocumentWithOptions(
                DetachFromCentralOption.DetachAndPreserveWorksets,
                worksetConfiguration,
                app);

        public static Document OpenTransmitted(this ModelPath modelPath, Application app) =>
            modelPath.OpenDocumentWithOptions(
                DetachFromCentralOption.ClearTransmittedSaveAsNewCentral,
                new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets),
                app);

        private static Document OpenDocumentWithOptions(this ModelPath modelPath,
            DetachFromCentralOption detachOption,
            WorksetConfiguration worksetConfiguration,
            Application app)
        {
            OpenOptions openOptions = new OpenOptions() { DetachFromCentralOption = detachOption };
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);
            return app.OpenDocumentFile(modelPath, openOptions);
        }
    }
}