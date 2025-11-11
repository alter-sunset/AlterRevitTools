using Application = Autodesk.Revit.ApplicationServices.Application;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.BatchExport.Utils.Extensions;

public static class ModelPathExtensions
{
    [UsedImplicitly]
    public static Document OpenAsIs(this ModelPath modelPath,
        Application app,
        WorksetConfiguration worksetConfiguration)
    {
        return modelPath.OpenDocumentWithOptions(DetachFromCentralOption.DoNotDetach,
            worksetConfiguration,
            app);
    }

    public static Document OpenDetached(this ModelPath modelPath,
        Application app,
        WorksetConfiguration worksetConfiguration)
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
    
    /// <summary>
    ///     Get WorksetConfiguration with closed worksets that match given prefixes
    /// </summary>
    public static WorksetConfiguration CloseWorksets(this ModelPath modelPath, params string[] prefixes)
    {
        if (prefixes is null || prefixes.Length == 0)
        {
            return new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);
        }

        // problem occurs if centralModel can't be found
        try
        {
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.CloseAllWorksets);

            IList<WorksetId> worksetIds =
            [
                .. WorksharingUtils.GetUserWorksetInfo(modelPath)
                    .Where(wp => !prefixes.Any(wp.Name.StartsWith))
                    .Select(wp => wp.Id)
            ];

            worksetConfiguration.Open(worksetIds);

            return worksetConfiguration;
        }
        catch
        {
            // just return default worksetConfiguration
            return new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);
        }
    }
    
    public static void UnloadRevitLinks(this ModelPath filePath, string folder, bool isSameFolder = true)
    {
        if (!TryGetTransmissionData(filePath, out TransmissionData transData)) return;

        ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();

        foreach (ElementId refId in externalReferences)
        {
            ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
            if (extRef.ExternalFileReferenceType is not ExternalFileReferenceType.RevitLink) continue;
            
            string name = Path.GetFileName(extRef.GetPath().CentralServerPath);
            if (name is null) continue;

            (ModelPath path, PathType pathType) = isSameFolder
                ? (new FilePath(Path.Combine(folder, name)), PathType.Absolute)
                : (extRef.GetPath(), extRef.PathType);

            transData.SetDesiredReferenceData(refId, path, pathType, false);
        }

        transData.IsTransmitted = true;

        TransmissionData.WriteTransmissionData(filePath, transData);
    }

    public static void ReplaceLinks(this ModelPath filePath, Dictionary<string, string> oldNewFilePairs)
    {
        if (!TryGetTransmissionData(filePath, out TransmissionData transData)) return;

        ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();

        foreach (ElementId refId in externalReferences)
        {
            ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);
            if (extRef.ExternalFileReferenceType is not ExternalFileReferenceType.RevitLink) continue;

            ModelPath modelPath = extRef.GetAbsolutePath();
            string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

            if (!oldNewFilePairs.TryGetValue(path, out string newFile)) continue;

            ModelPath newPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(newFile);

            try
            {
                transData.SetDesiredReferenceData(refId, newPath, PathType.Absolute, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        transData.IsTransmitted = true;

        TransmissionData.WriteTransmissionData(filePath, transData);
    }
    
    private static bool TryGetTransmissionData(ModelPath filePath, out TransmissionData transData)
    {
        transData = TransmissionData.ReadTransmissionData(filePath);

        if (transData is not null) return true;

        TaskDialog.Show(Resources.Strings.Error, Resources.Strings.NoTransDataAlert);

        return false;
    }
}