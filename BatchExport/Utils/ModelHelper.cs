﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AlterTools.BatchExport.Utils
{
    public static class ModelHelper
    {
        /// <summary>
        /// Get WorksetConfiguration with closed worksets that match given prefixes
        /// </summary>
        public static WorksetConfiguration CloseWorksetsWithLinks(this ModelPath modelPath, params string[] prefixes)
        {
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);

            if (0 == prefixes.Length) return worksetConfiguration;

            List<WorksetId> worksetIds = WorksharingUtils.GetUserWorksetInfo(modelPath)
                                                         .Where(wp => prefixes.Any(wp.Name.StartsWith))
                                                         .Select(wp => wp.Id)
                                                         .ToList();

            if (0 < worksetIds.Count) worksetConfiguration.Close(worksetIds);

            return worksetConfiguration;
        }

        /// <summary>
        /// Checks whether given view has no visible objects 
        /// </summary>
        /// <param name="element">View</param>
        /// <returns>true if view is empty</returns>
        public static bool IsViewEmpty(this Document doc, Element element)
        {
            View3D view = element as View3D;

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
        /// Relinquish ownership of all possible elements in the doc
        /// </summary>
        public static void FreeTheModel(this Document doc)
        {
            try
            {
                WorksharingUtils.RelinquishOwnership(doc, new RelinquishOptions(true), new TransactWithCentralOptions());
            }
            catch { }
        }

        /// <summary>
        /// Delete all possible links from the doc 
        /// </summary>
        public static void DeleteAllLinks(this Document doc)
        {
            ICollection<ElementId> ids = ExternalFileUtils.GetAllExternalFileReferences(doc);

            if (0 == ids.Count) return;

            using Transaction tr = new(doc, "Delete all Links");

            tr.Start();
            tr.SuppressAlert();

            foreach (ElementId id in ids)
            {
                try
                {
                    doc.Delete(id);
                }
                catch { }
            }

            tr.Commit();
        }

        /// <summary>
        /// Returns string with MD5 Hash of given file
        /// </summary>
        public static string MD5Hash(this string fileName)
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
        /// Open all worksets in a doc in a very crippled way
        /// </summary>
        public static void OpenAllWorksets(this Document doc)
        {
            // List of all user worksets
            IList<Workset> collectorWorkset = new FilteredWorksetCollector(doc)
                                                  .OfKind(WorksetKind.UserWorkset)
                                                  .ToWorksets();

            ElementId typeId = new FilteredElementCollector(doc)
                                   .WhereElementIsElementType()
                                   .OfClass(typeof(CableTrayType))
                                   .ToElementIds()
                                   .FirstOrDefault();

            ElementId levelId = new FilteredElementCollector(doc)
                                    .OfClass(typeof(Level))
                                    .ToElementIds()
                                    .FirstOrDefault();

            if (null == typeId) return;
            if (null == levelId) return;

            using Transaction tr = new(doc, "Open worksets");

            tr.Start();

            // Create a temporary cable tray
            CableTray ct = CableTray.Create(doc, typeId, new XYZ(0, 0, 0), new XYZ(0, 0, 1), levelId);

            foreach (Workset workset in collectorWorkset)
            {
                if (workset.IsOpen) continue;

                // Change the workset of the cable tray
                Parameter wsParam = ct.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                if (null != wsParam
                    && !wsParam.IsReadOnly)
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
        public static void YesNoTaskDialog(string msg, Action action)
        {
            TaskDialogResult result = TaskDialog.Show("Ошибка", msg, TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
            if (TaskDialogResult.Yes == result)
            {
                action?.Invoke();
            }
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

                    if (0 == previousCount) break;

                    using (Transaction tr = new(doc, "Purge unused"))
                    {
                        tr.Start();

                        doc.Delete(unusedElements);

                        tr.Commit();
                    }
                } while (0 < previousCount);
            }
            catch { }
        }
#endif

#if R22_OR_GREATER
        public static void RemoveEmptyWorksets(this Document doc)
        {
            List<WorksetId> worksets = new FilteredWorksetCollector(doc)
                                           .OfKind(WorksetKind.UserWorkset)
                                           .ToWorksetIds()
                                           .Where(doc.IsWorksetEmpty)
                                           .ToList();

            using (Transaction tr = new(doc))
            {
                tr.Start("Remove empty worksets");

                worksets.ForEach(workset => WorksetTable.DeleteWorkset(doc, workset, new DeleteWorksetSettings()));

                tr.Commit();
            }
        }

        private static bool IsWorksetEmpty(this Document doc, WorksetId workset)
        {
            return !new FilteredElementCollector(doc)
                        .WherePasses(new ElementWorksetFilter(workset))
                        .Any();
        }
#endif

        /// <summary>
        /// Return parameter value as string with correct null check
        /// </summary>
        public static string GetValueString(this Parameter param)
        {
            return param is null || param.AsValueString() is null
                ? string.Empty
                : param.AsValueString().Trim();
        }

        public static bool IsPhysicalElement(this Element el)
        {
            if (null == el.Category) return false;
            if (el.ViewSpecific) return false;
            // exclude specific unwanted categories
#if R24_OR_GREATER
            if (((BuiltInCategory)e.Category.Id.Value) == BuiltInCategory.OST_HVAC_Zones) return false;
#else
            if (BuiltInCategory.OST_HVAC_Zones == ((BuiltInCategory)el.Category.Id.IntegerValue)) return false;
#endif
            return CategoryType.Model == el.Category.CategoryType
                && el.Category.CanAddSubcategory;
        }
    }
}