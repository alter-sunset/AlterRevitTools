using System.IO;
using System.Reflection;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils;
using AlterTools.Utils.Extensions;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atExportModel;

public class ExternalEventHandler : RevitEventWrapper<IConfigExportMultiple>
{
    public override void Execute(UIApplication uiApp, IConfigExportMultiple args)
    {
        if (args is null) return;
        using Application app = uiApp.Application;
        // using ErrorSuppressor errorSuppressor = new(uiApp);
        ConfigExportSingle config = new(args);

        foreach (string file in args.InputFiles)
        {
            config.FileName = file;
            ProcessModel(app, config);
        }
    }

    private static void Debug(string arg)
    {
        using TaskDialog taskDialog = new("debug");
        taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
        taskDialog.Id = "debug";
        taskDialog.MainContent = arg;

        taskDialog.Show();
    }

    private static void ProcessModel(Application app, ConfigExportSingle config)
    {
        bool dontOpen = !config.ExportNWC &&
                        !config.ExportIFC &&
                        !config.CleanModel &&
                        config.ExportRVT &&
                        config.AsTransmit;

        // 1. Check if file should be opened at all -- forgot about RevitServer, maybe need to add external worker?
        if (dontOpen)
        {
            string transFilePath = Path.Combine(config.FolderPathRVT, Path.GetFileName(config.FileName));
            File.Copy(config.FileName, transFilePath, true);

            // TODO: add copying mechanism for RS

            using ModelPath transModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(transFilePath);
            transModelPath.UnloadRevitLinks(config.FolderPathRVT);
            return;
        }

        // open model
        using Document doc = OpenDocument(config.FileName, app, out bool isWorkshared);
        if (doc is null) return;

        string modelName = doc.Title.RemoveDetach();

        // export nwc
        if (config.ExportNWC)
        {
            using NavisworksExportOptions options = GetNWCExportOptions(config, doc);
            doc.Export(config.FolderPathNWC, modelName, options);
        }

        // export ifc and rollback transaction
        if (config.ExportIFC)
        {
            using Transaction tr = new(doc);
            tr.Start(Resources.Strings.IFCTitle);

            using IFCExportOptions options = GetIFCExportOptions(config, doc);
            doc.Export(config.FolderPathIFC, modelName, options);

            tr.RollBack();
        }

        if (config.ExportRVT)
        {
            using ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(config.FileName);
            using TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
            // clean
            if (config.CleanModel)
            {
                //process document
            }

            // saveAs -- currently only transmit
            // TODO: Add newCentral
            SaveDocument(doc, modelName, isWorkshared, transData);
            //do stuff on close
            return;
        }

        CloseDocument(doc);
    }

    private static Document OpenDocument(string file, Application app, out bool isWorkshared)
    {
        try
        {
            using BasicFileInfo fileInfo = BasicFileInfo.Extract(file);
            isWorkshared = fileInfo.IsWorkshared;
            using ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(file);

            using TransmissionData trData =
                File.Exists(fileInfo.CentralPath) // ensure that central model exists and reachable
                    ? TransmissionData.ReadTransmissionData(modelPath)
                    : null;

            // bool transmitted = trData is { IsTransmitted: true };

            WorksetConfiguration worksetConfiguration;

            if (!isWorkshared)
            {
                worksetConfiguration = null;
            }
            // else if (!transmitted && iConfig.WorksetPrefixes.Length != 0)
            // {
            //     worksetConfiguration = modelPath.CloseWorksets(app, iConfig.WorksetPrefixes);
            // } // need to add worksetClosing method or smthng
            else
            {
                worksetConfiguration = new WorksetConfiguration();
            }

            if (worksetConfiguration is null) return app.OpenDocumentFile(file);
            return modelPath.OpenDetached(app, worksetConfiguration);
        }
        catch
        {
            isWorkshared = false;
            return null;
        }
    }

    private static NavisworksExportOptions GetNWCExportOptions(ConfigExportSingle config, Document doc)
    {
        IConfigNWC configNWC = config.ConfigNWC;
        NavisworksExportOptions options = new()
        {
            ConvertElementProperties = configNWC.ConvertElementProperties,
            DivideFileIntoLevels = configNWC.DivideFileIntoLevels,
            ExportElementIds = configNWC.ExportElementIds,
            ExportLinks = configNWC.ExportLinks,
            ExportParts = configNWC.ExportParts,
            ExportRoomAsAttribute = configNWC.ExportRoomAsAttribute,
            ExportRoomGeometry = configNWC.ExportRoomGeometry,
            ExportUrls = configNWC.ExportUrls,
            FindMissingMaterials = configNWC.FindMissingMaterials,
            Coordinates = configNWC.Coordinates,
            Parameters = configNWC.Parameters,
            ExportScope = configNWC.ExportScope,

#if R20_OR_GREATER
            ConvertLights = configNWC.ConvertLights,
            ConvertLinkedCADFormats = configNWC.ConvertLinkedCADFormats,
            FacetingFactor = configNWC.FacetingFactor,
#endif
        };

        if (configNWC.ExportScope == NavisworksExportScope.View && doc.DoesViewExist(config.ViewName))
        {
            options.ExportScope = NavisworksExportScope.View;
            options.ViewId = new FilteredElementCollector(doc)
                .OfClass(typeof(View3D))
                .FirstOrDefault(el => el.Name == config.ViewName && !((View3D)el).IsTemplate)
                .Id;
        }
        else
        {
            options.ExportScope = NavisworksExportScope.Model;
        }

        return options;
    }

    private static IFCExportOptions GetIFCExportOptions(ConfigExportSingle config, Document doc)
    {
        IConfigIFC configIFC = config.ConfigIFC;
        IConfigIFCAdditionalFields configIFCAdd = config.ConfigIFCAdditionalFields;
        IFCExportOptions options = new()
        {
            ExportBaseQuantities = configIFC.ExportBaseQuantities,
            FamilyMappingFile = configIFC.FamilyMappingFile,
            FileVersion = configIFC.FileVersion,
            SpaceBoundaryLevel = (int)configIFC.SpaceBoundaryLevel,
            WallAndColumnSplitting = configIFC.WallAndColumnSplitting
        };

        Dictionary<string, string> additionals = configIFCAdd.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(
                f => f.Name,
                f => f.GetValue(configIFCAdd)?.ToString() ?? string.Empty
            );

        foreach (KeyValuePair<string, string> field in additionals)
        {
            options.AddOption(field.Key, field.Value);
        }

        // add logic for view, and don't forget to add to Window
        // if (configIFC.ExportScopeView && doc.DoesViewExist(configIFC.ViewName))
        // {
        //     options.FilterViewId = new FilteredElementCollector(doc)
        //         .OfClass(typeof(View3D))
        //         .FirstOrDefault(el => el.Name == configIFC.ViewName && !((View3D)el).IsTemplate)
        //         .Id;
        // }

        return options;
    }

    private static void SaveDocument(Document doc, string fileDetachedPath, bool isWorkshared,
        TransmissionData transData)
    {
        using SaveAsOptions saveOptions = new();
        saveOptions.OverwriteExistingFile = true;
        saveOptions.MaximumBackups = 1;

        if (isWorkshared)
        {
            using WorksharingSaveAsOptions worksharingOptions = new();
            worksharingOptions.SaveAsCentral = true;

            if (transData is not null && transData.IsTransmitted)
            {
                worksharingOptions.ClearTransmitted = true;
            }

            worksharingOptions.OpenWorksetsDefault = SimpleWorksetConfiguration.AskUserToSpecify;
            saveOptions.SetWorksharingOptions(worksharingOptions);
        }

        try
        {
            using ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
            doc.SaveAs(modelPath, saveOptions);
        }
        catch
        {
            // ignored
        }
    }

    private static void CloseDocument(Document doc)
    {
        if (doc is null) return;

        try
        {
            doc.FreeTheModel();
        }
        finally
        {
            doc.Close(false);
            doc.Dispose();
        }
    }
}