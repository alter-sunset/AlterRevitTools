using System.Reflection;
using AlterTools.BatchExport.Resources;
using Autodesk.Revit.DB.Electrical;

namespace AlterTools.BatchExport.Utils.Extensions;

public static class DocumentExtensions
{
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

            return !collector.Where(el => el.Category != null
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
    ///     Delete all possible links from the doc
    /// </summary>
    public static void DeleteAllLinks(this Document doc)
    {
        ICollection<ElementId> ids = ExternalFileUtils.GetAllExternalFileReferences(doc);

        if (ids.Count == 0) return;

        using Transaction tr = new(doc, Strings.RemoveAllLinks);

        tr.Start();

        using FailureHandlingOptions failOpt = tr.GetFailureHandlingOptions();
        failOpt.SetFailuresPreprocessor(new CopyWatchAlertSuppressor());
        tr.SetFailureHandlingOptions(failOpt);

        foreach (ElementId id in ids)
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

            if (wsParam is { IsReadOnly: false })
            {
                wsParam.Set(workset.Id.IntegerValue);
            }

            // Show the cable tray to open the workset
            new UIDocument(doc).ShowElements(ct.Id);
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
#else
            List<ElementId> unusedElements = doc.GetUnusedElements();
#endif
            previousCount = unusedElements.Count;

            if (previousCount == 0) break;

            using Transaction tr = new(doc, Strings.PurgeUnused);
            tr.Start();

#if R24_OR_GREATER
            doc.Delete(unusedElements);
#else
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
#endif

            tr.Commit();
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

    private static List<ElementId> GetUnusedElements(this Document doc)
    {
        return GetUnusedAssets(doc, "GetUnusedAppearances")
            .Concat(GetUnusedAssets(doc, "GetUnusedImportCategories"))
            .Concat(GetUnusedAssets(doc, "GetUnusedFamilies"))
            .Concat(GetUnusedAssets(doc, "GetUnusedLinkSymbols"))
            .Concat(GetUnusedAssets(doc, "GetUnusedMaterials"))
            .Concat(GetUnusedAssets(doc, "GetUnusedStructures"))
            .Concat(GetUnusedAssets(doc, "GetUnusedSymbols"))
            .Concat(GetUnusedAssets(doc, "GetUnusedThermals"))
            .ToList();
    }
#endif
}