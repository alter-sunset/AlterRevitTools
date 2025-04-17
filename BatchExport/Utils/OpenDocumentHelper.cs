using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace AlterTools.BatchExport.Utils
{
    public static class OpenDocumentHelper
    {
        public static Document OpenAsIs(this ModelPath modelPath, Application app, WorksetConfiguration worksetConfiguration)
        {
            return modelPath.OpenDocumentWithOptions(DetachFromCentralOption.DoNotDetach,
                                                     worksetConfiguration,
                                                     app);
        }

        public static Document OpenDetached(this ModelPath modelPath, Application app, WorksetConfiguration worksetConfiguration)
        {
            return modelPath.OpenDocumentWithOptions(DetachFromCentralOption.DetachAndPreserveWorksets,
                                                     worksetConfiguration,
                                                     app);
        }

        public static Document OpenTransmitted(this ModelPath modelPath, Application app)
        {
            return modelPath.OpenDocumentWithOptions(DetachFromCentralOption.ClearTransmittedSaveAsNewCentral,
                                                     new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets),
                                                     app);
        }

        private static Document OpenDocumentWithOptions(this ModelPath modelPath,
                                                        DetachFromCentralOption detachOption,
                                                        WorksetConfiguration worksetConfiguration,
                                                        Application app)
        {
            OpenOptions openOptions = new() { DetachFromCentralOption = detachOption };
            openOptions.SetOpenWorksetsConfiguration(worksetConfiguration);

            return app.OpenDocumentFile(modelPath, openOptions);
        }

        public static Document OpenDocument(Application app, string filePath, out bool isWorkshared)
        {
            Document doc = null;
            isWorkshared = false;

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

                    doc = modelPath.OpenDetached(app,
                                                 new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets));

                    isWorkshared = true;
                }
            }
            catch
            {
                // ignored
            }

            return doc;
        }
    }
}