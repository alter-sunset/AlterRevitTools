using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Utils;

public static class ModelHelper
{
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

            IList<WorksetId> worksetIds = WorksharingUtils.GetUserWorksetInfo(modelPath)
                .Where(wp => !prefixes.Any(wp.Name.StartsWith))
                .Select(wp => wp.Id)
                .ToList();

            worksetConfiguration.Open(worksetIds);

            return worksetConfiguration;
        }
        catch
        {
            // just return default worksetConfiguration
            return new WorksetConfiguration(WorksetConfigurationOption.OpenAllWorksets);
        }
    }

    public static bool DoesViewExist(this Document doc, string viewName)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(View3D))
            .Any(el => el.Name == viewName && !((View3D)el).IsTemplate);
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
            WorksharingUtils.RelinquishOwnership(doc, new RelinquishOptions(true),
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

        using Transaction tr = new(doc, "Delete all Links");

        tr.Start();
        tr.SuppressAlert();

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
    ///     Returns string with MD5 Hash of given file
    /// </summary>
    public static string GetMd5Hash(this string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return null;

        if (!File.Exists(fileName)) return null;

        using MD5 md5 = MD5.Create();

        try
        {
            using FileStream stream = File.OpenRead(fileName);
            return Convert.ToBase64String(md5.ComputeHash(stream));
        }
        catch
        {
            return null;
        }
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

        using Transaction tr = new(doc, "Open worksets");
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

    private static void SuppressAlert(this Transaction tr)
    {
        FailureHandlingOptions failOpt = tr.GetFailureHandlingOptions();
        failOpt.SetFailuresPreprocessor(new CopyWatchAlertSuppressor());
        tr.SetFailureHandlingOptions(failOpt);
    }

#if R24_OR_GREATER
        public static void PurgeAll(this Document doc)
        {
            try
            {
                int previousCount;

                do
                {
                    HashSet<ElementId> unusedElements = doc.GetUnusedElements(new HashSet<ElementId>())
                        .Where(el => doc.GetElement(el) is not null
                                     && doc.GetElement(el) is not RevitLinkType)
                        .ToHashSet();

                    previousCount = unusedElements.Count;

                    if (previousCount == 0) break;

                    using Transaction tr = new(doc, "Purge unused");
                    tr.Start();

                    doc.Delete(unusedElements);

                    tr.Commit();
                } while (0 < previousCount);
            }
            catch
            {
                // ignored
            }
        }
#endif

    /// <summary>
    ///     Return parameter value as string with correct null check
    /// </summary>
    public static string GetValueString(this Parameter param)
    {
        return param?.AsValueString() is null
            ? string.Empty
            : param.AsValueString().Trim();
    }

    public static bool IsPhysicalElement(this Element el)
    {
        if (el.Category is null) return false;

        if (el.ViewSpecific) return false;
        // exclude specific unwanted categories
#if R24_OR_GREATER
        if ((BuiltInCategory)el.Category.Id.Value is BuiltInCategory.OST_HVAC_Zones) return false;
#else
        if ((BuiltInCategory)el.Category.Id.IntegerValue is BuiltInCategory.OST_HVAC_Zones) return false;
#endif
        return el.Category.CategoryType is CategoryType.Model && el.Category.CanAddSubcategory;
    }

#if R22_OR_GREATER
    public static void RemoveEmptyWorksets(this Document doc)
    {
        List<WorksetId> worksets = new FilteredWorksetCollector(doc)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksetIds()
            .Where(doc.IsWorksetEmpty)
            .ToList();

        using Transaction tr = new(doc);
        tr.Start("Remove empty worksets");

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
}