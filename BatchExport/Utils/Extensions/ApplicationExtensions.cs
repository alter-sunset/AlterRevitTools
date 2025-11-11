using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.BatchExport.Utils.Extensions;

public static class ApplicationExtensions
{
    public static Document OpenDocument(this Application app, string filePath, out bool isWorkshared)
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