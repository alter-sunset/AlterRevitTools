using System.IO;
using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils.Extensions;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace AlterTools.atExportModel.Utils;

public static class UtilsRVT
{
    // TODO: fill it with stuff
    public static void CleanTheModel(Document doc, IConfigClean config)
    {
        if (config is null) return;

        if (config.UnloadLinks)
        {
            doc.UnloadAllLinks();
        }

        if (config.RemoveLinkedRvt)
        {
            doc.DeleteRVTLinks();
        }

        if (config.RemoveLinkedCad)
        {
            doc.DeleteCADLinks();
            // remove imports and stuff?
        }

        if (config.RemoveOrphanedRooms)
        {
            doc.RemoveOrphanedRooms();
        }

        if (config.Purge)
        {
            doc.PurgeAll();
        }

        if (config.RemoveSheets)
        {
            doc.RemoveAllSheets();
        }

        if (config.RemoveViews)
        {
            RemoveViewsNotOnSheets(doc, config.ConfigRemoveViews);
        }

        if (config.RemoveEmptyWorksets)
        {
            doc.RemoveEmptyWorksets();
        }
    }

    public static void CleanupAndClose(Document doc,
        string fileDetachedPath,
        bool isWorkshared,
        RvtExportMode exportMode)
    {
        try
        {
            doc.FreeTheModel();
        }
        catch
        {
            // ignored
        }
        finally
        {
            doc?.Close();
        }

        // RevitServer path, no cleanup needed
        if (fileDetachedPath.StartsWith("RSN")) return;

        if (isWorkshared)
        {
            if (exportMode == RvtExportMode.Transmit)
            {
                using ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(fileDetachedPath);
                UpdateTransmissionData(modelPath);
            }

            string backupFolderPath = fileDetachedPath.Replace(".rvt", "_backup");
            if (!Directory.Exists(backupFolderPath)) return;

            Directory.Delete(backupFolderPath, true);
        }

        for (int i = 1; i <= 3; i++)
        {
            string versionedFilePath = fileDetachedPath.Replace(".rvt", $".{i:D4}.rvt");
            if (!File.Exists(versionedFilePath)) continue;

            File.Delete(versionedFilePath);
        }
    }

    private static void UpdateTransmissionData(ModelPath modelPath)
    {
        using TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
        if (transData is null) return;

        transData.IsTransmitted = true;

        TransmissionData.WriteTransmissionData(modelPath, transData);
    }

    private static void RemoveViewsNotOnSheets(Document doc, ConfigRemoveViews config)
    {
        using Transaction tr = new(doc, "Remove Views Not On Sheets");
        tr.Start();

        // ---------------------------------------------------------
        // Get all views that are currently placed on sheets.
        // ---------------------------------------------------------

        HashSet<ElementId> viewsOnSheets = [];

        // Normal views, legends, sections, elevations, etc.
        // placed using Viewport.
        IEnumerable<Viewport> viewports = new FilteredElementCollector(doc)
            .OfClass(typeof(Viewport))
            .OfType<Viewport>();

        foreach (Viewport viewport in viewports)
        {
            viewsOnSheets.Add(viewport.ViewId);
        }

        // Schedules are placed using ScheduleSheetInstance,
        // rather than Viewport.
        IEnumerable<ScheduleSheetInstance> scheduleInstances = new FilteredElementCollector(doc)
            .OfClass(typeof(ScheduleSheetInstance))
            .OfType<ScheduleSheetInstance>();

        foreach (ScheduleSheetInstance scheduleInstance in scheduleInstances)
        {
            viewsOnSheets.Add(scheduleInstance.ScheduleId);
        }

        // ---------------------------------------------------------
        // Find views matching the configuration.
        // ---------------------------------------------------------

        List<ElementId> viewsToDelete = new FilteredElementCollector(doc)
            .OfClass(typeof(View))
            .OfType<View>()
            .Where(view => !view.IsTemplate)
            .Where(view => ShouldRemoveView(view, config))
            .Where(view => !viewsOnSheets.Contains(view.Id))
            .Select(view => view.Id)
            .ToList();

        // ---------------------------------------------------------
        // Delete.
        // ---------------------------------------------------------

        if (viewsToDelete.Any())
        {
            doc.Delete(viewsToDelete);
        }

        tr.Commit();
    }

    private static bool ShouldRemoveView(View view, ConfigRemoveViews config)
    {
        // All == true means all supported view categories.
        if (config.All is true) return true;

        // All == false explicitly disables all categories.
        if (config.All is false) return false;

        // All == null means use individual configuration flags.
        return view.ViewType switch
        {
            ViewType.ThreeD => config.ThreeDViews,
            ViewType.AreaPlan => config.AreaPlans,
            ViewType.CeilingPlan => config.CeilingPlans,
            ViewType.Detail => config.Details,
            ViewType.DraftingView => config.DraftingViews,
            ViewType.Elevation => config.Elevations,
            ViewType.FloorPlan => config.FloorPlans,
            ViewType.ColumnSchedule => config.ColumnSchedules,
            ViewType.Legend => config.Legends,
            ViewType.Rendering => config.Renderings,
            ViewType.Schedule => config.Schedules,
            ViewType.Section => config.Sections,
            ViewType.EngineeringPlan => config.EngineeringPlans,
            ViewType.Walkthrough => config.Walkthroughs,
            _ => false
        };
    }
}