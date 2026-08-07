using System.IO;
using System.Reflection;
using AlterTools.Resources;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using JetBrains.Annotations;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.Utils.Extensions;

public static class DocumentExtensions
{
    public static Document OpenDocument(string file, Application app, out bool isWorkshared)
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

    public static void SaveDocument(Document doc, string fileDetachedPath, bool isWorkshared,
        [CanBeNull] TransmissionData transData)
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

    public static void CloseDocument(Document doc)
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

    public static bool DoesViewExist(this Document doc, string viewName)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(View3D))
            .Any(el => el.Name == viewName
                       && !((View3D)el).IsTemplate);
    }

    /// <summary>
    ///     Checks whether given view has no visible objects
    /// </summary>
    /// <param name="doc">Document to inspect</param>
    /// <param name="element">View to check</param>
    /// <returns>true if view is empty</returns>
    public static bool IsViewEmpty(this Document doc, Element element)
    {
        if (element is not View3D view) return true;

        try
        {
            using FilteredElementCollector collector = new(doc, view.Id);

            return !collector.Where(el => el.Category is not null
                                          && el.GetType() != typeof(RevitLinkInstance))
                .Any(el => el.CanBeHidden(view));
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    ///     Relinquish ownership of all possible elements in the doc
    /// </summary>
    public static void FreeTheModel(this Document doc)
    {
        try
        {
            WorksharingUtils.RelinquishOwnership(doc,
                new RelinquishOptions(true),
                new TransactWithCentralOptions());
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    ///     Unload all possible links from the doc
    /// </summary>
    public static void UnloadAllLinks(this Document doc)
    {
        RevitLinkType[] links = new FilteredElementCollector(doc)
            .OfClass(typeof(RevitLinkType))
            .Cast<RevitLinkType>()
            .ToArray();

        if (links.Length == 0) return;

        foreach (RevitLinkType link in links)
        {
            try
            {
                link.UnloadLocally(null);
            }
            catch
            {
                // ignore
            }
        }
    }

    /// <summary>
    /// Delete all possible links from the doc
    /// </summary>
    /// <param name="doc">Document to wirk with</param>
    /// <param name="noImports">true if imported instances should be left intact.</param>
    public static void DeleteAllLinks(this Document doc, bool noImports = true)
    {
        ICollection<ElementId> ids = ExternalFileUtils.GetAllExternalFileReferences(doc);
        ICollection<ElementId> imgs = [];
        ICollection<ElementId> importedCads = [];
        if (!noImports)
        {
            imgs = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfCategory(BuiltInCategory.OST_RasterImages)
                .ToElementIds();
            importedCads = new FilteredElementCollector(doc)
                .WhereElementIsElementType()
                .OfClass(typeof(CADLinkType))
                .ToElementIds();
        }

        IEnumerable<ElementId> all = ids.Concat(imgs).Concat(importedCads);

        if (ids.Count == 0 && imgs.Count == 0 && importedCads.Count == 0) return;

        using Transaction tr = new(doc, Strings.RemoveAllLinks);

        tr.Start();

        using FailureHandlingOptions failOpt = tr.GetFailureHandlingOptions();
        failOpt.SetFailuresPreprocessor(new CopyWatchAlertSuppressor());
        tr.SetFailureHandlingOptions(failOpt);

        foreach (ElementId id in all)
        {
            try
            {
                doc.Delete(id);
            }
            catch
            {
                // ignored
            }
        }

        tr.Commit();
    }

    /// <summary>
    ///     Open all worksets in a doc in a very crippled way
    /// </summary>
    public static void OpenAllWorksets(this Document doc)
    {
        ElementId typeId = new FilteredElementCollector(doc)
            .WhereElementIsElementType()
            .OfClass(typeof(CableTrayType))
            .ToElementIds()
            .FirstOrDefault();
        if (typeId is null) return;

        ElementId levelId = new FilteredElementCollector(doc)
            .OfClass(typeof(Level))
            .ToElementIds()
            .FirstOrDefault();
        if (levelId is null) return;

        using UIDocument uiDoc = new(doc);

        // List of all user worksets
        IList<Workset> collectorWorkset = new FilteredWorksetCollector(doc)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets();

        using Transaction tr = new(doc, Strings.OpenWorksets);
        tr.Start();

        // Create a temporary cable tray
        CableTray ct = CableTray.Create(doc, typeId, new XYZ(0, 0, 0), new XYZ(0, 0, 1), levelId);

        foreach (Workset workset in collectorWorkset)
        {
            if (workset.IsOpen) continue;

            // Change the workset of the cable tray
            Parameter wsParam = ct.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

            if (wsParam is { IsReadOnly: false }) wsParam.Set(workset.Id.IntegerValue);

            // Show the cable tray to open the workset
            uiDoc.ShowElements(ct.Id);
        }

        // Delete the temporary cable tray
        doc.Delete(ct.Id);

        tr.Commit();
    }


#if R22_OR_GREATER
    public static void RemoveEmptyWorksets(this Document doc)
    {
        List<WorksetId> worksets =
        [
            .. new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset)
                .ToWorksetIds()
                .Where(doc.IsWorksetEmpty)
        ];

        using Transaction tr = new(doc);
        tr.Start(Strings.RemoveEmptyWorksets);

        worksets.ForEach(workset => WorksetTable.DeleteWorkset(doc, workset, new DeleteWorksetSettings()));

        tr.Commit();
    }

    private static bool IsWorksetEmpty(this Document doc, WorksetId workset)
    {
        return !new FilteredElementCollector(doc)
            .WherePasses(new ElementWorksetFilter(workset))
            .Any();
    }
#endif


    public static void PurgeAll(this Document doc)
    {
        int previousCount;

        do
        {
#if R24_OR_GREATER
            HashSet<ElementId> unusedElements =
            [
                .. doc.GetUnusedElements(new HashSet<ElementId>())
                    .Where(el => doc.GetElement(el) is not null
                                 && doc.GetElement(el) is not RevitLinkType)
            ];
            previousCount = unusedElements.Count;
            if (previousCount == 0) break;

            using Transaction tr = new(doc, Strings.PurgeUnused);
            tr.Start();

            doc.Delete(unusedElements);
            tr.Commit();
#else
            HashSet<ElementId> unusedElements = doc.GetUnusedElements();
            previousCount = unusedElements.Count;
            if (previousCount == 0) break;

            using Transaction tr = new(doc, Strings.PurgeUnused);
            tr.Start();

            foreach (ElementId id in unusedElements)
            {
                try
                {
                    doc.Delete(id);
                }
                catch
                {
                    // ignored
                }
            }

            tr.Commit();
#endif
        } while (0 < previousCount);
    }

#if R24_OR_GREATER
#else
    private static ICollection<ElementId> GetUnusedAssets(Document doc, string methodName)
    {
        MethodInfo method = typeof(Document)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method is null) return [];
        return (ICollection<ElementId>)method.Invoke(doc, null);
    }

    private static HashSet<ElementId> GetUnusedElements(this Document doc)
    {
        return
        [
            ..GetUnusedAssets(doc, "GetUnusedAppearances")
                .Concat(GetUnusedAssets(doc, "GetUnusedFamilies"))
                .Concat(GetUnusedAssets(doc, "GetUnusedImportCategories"))
                .Concat(GetUnusedAssets(doc, "GetUnusedLinkSymbols"))
                .Concat(GetUnusedAssets(doc, "GetUnusedMaterials"))
                .Concat(GetUnusedAssets(doc, "GetUnusedStructures"))
                .Concat(GetUnusedAssets(doc, "GetUnusedSymbols"))
                .Concat(GetUnusedAssets(doc, "GetUnusedThermals"))
                .Where(el => doc.GetElement(el) is not null
                             && doc.GetElement(el) is not RevitLinkType)
        ];
    }
#endif
}