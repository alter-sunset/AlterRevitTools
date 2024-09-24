using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace VLS.BatchExportNet.Utils
{
    static class ModelHelper
    {
        /// <summary>
        /// Get WorksetConfiguration with closed worksets that match given prefixes
        /// </summary>
        internal static WorksetConfiguration CloseWorksetsWithLinks(this ModelPath modelPath, params string[] prefixes)
        {
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);

            IList<WorksetPreview> worksets = WorksharingUtils.GetUserWorksetInfo(modelPath);
            List<WorksetId> worksetIds = [];

            foreach (WorksetPreview worksetPreview in worksets)
            {
                if (prefixes.Any(worksetPreview.Name.StartsWith))
                {
                    worksetIds.Add(worksetPreview.Id);
                }
            }

            worksetConfiguration.Close(worksetIds);
            return worksetConfiguration;
        }
        /// <summary>
        /// Checks whether given view has no visible objects 
        /// </summary>
        /// <param name="element">View</param>
        /// <returns>true if view is empty</returns>
        internal static bool IsViewEmpty(this Document doc, Element element)
        {
            View3D view = element as View3D;
            try
            {
                using FilteredElementCollector collector = new(doc, view.Id);
                return !collector.Where(e => e.Category != null
                        && e.GetType() != typeof(RevitLinkInstance))
                    .Any(e => e.CanBeHidden(view));
            }
            catch
            {
                return true;
            }
        }
        /// <summary>
        /// Relinquish ownership of all possible elements in the document
        /// </summary>
        internal static void FreeTheModel(this Document doc)
        {
            RelinquishOptions relinquishOptions = new(true);
            TransactWithCentralOptions transactWithCentralOptions = new();
            try
            {
                WorksharingUtils.RelinquishOwnership(doc, relinquishOptions, transactWithCentralOptions);
            }
            catch { }
        }
        /// <summary>
        /// Delete all possible links from the document 
        /// </summary>
        internal static void DeleteAllLinks(this Document doc)
        {
            using Transaction t = new(doc);
            t.Start("Delete all Links");

            FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
            failOpt.SetFailuresPreprocessor(new CopyWatchAlertSwallower());
            t.SetFailureHandlingOptions(failOpt);

            ICollection<ElementId> ids = ExternalFileUtils.GetAllExternalFileReferences(doc);
            foreach (ElementId id in ids)
            {
                try
                {
                    doc.Delete(id);
                }
                catch { }
            }
            t.Commit();
        }
        /// <summary>
        /// Returns string with MD5 Hash of given file
        /// </summary>
        internal static string MD5Hash(this string fileName)
        {
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
        /// Open all worksets in a document in a very crippled way
        /// </summary>
        internal static void OpenAllWorksets(this Document doc)
        {
            using TransactionGroup tGroup = new(doc);
            using Transaction t = new(doc);
            tGroup.Start("Open All Worksets");

            //list of all worksets
            FilteredWorksetCollector collectorWorksett =
                new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset);

            foreach (Workset w in collectorWorksett.ToWorksets())
            {
                if (!w.IsOpen)
                {
                    t.Start("Open workset");//Creating temporary cable tray
                    ElementId typeID = new FilteredElementCollector(doc)
                        .WhereElementIsElementType()
                        .OfClass(typeof(CableTrayType))
                        .ToElementIds()
                        .First();
                    ElementId levelID = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .ToElementIds()
                        .First();
                    CableTray ct = CableTray.Create(doc, typeID, new XYZ(0, 0, 0), new XYZ(0, 0, 1), levelID);
                    ElementId elementId = ct.Id;

                    //Changing workset of cable tray to workset which we want to open
                    Parameter wsparam = ct.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                    if (wsparam != null && !wsparam.IsReadOnly) wsparam.Set(w.Id.IntegerValue);

                    List<ElementId> ids = [elementId];

                    //This command will actualy open workset
                    UIDocument uiDoc = new(doc);
                    uiDoc.ShowElements(ids);

                    //Delete temporary cable tray
                    doc.Delete(elementId);

                    t.Commit();
                }
            }
            tGroup.Assimilate();
        }
        /// <summary>
        /// Purge all unused elements in the Document
        /// </summary>
        internal static void PurgeAll(this Document doc)
        {
            try
            {
                int num = 0;
                for (; ; )
                {
                    HashSet<ElementId> hashSet = [];
                    foreach (ElementId elementId in doc.GetUnusedElements(new HashSet<ElementId>()))
                    {
                        Element element = doc.GetElement(elementId);
                        if (element != null && element is not RevitLinkType)
                        {
                            hashSet.Add(elementId);
                        }
                    }
                    if (hashSet.Count != num && hashSet.Count != 0)
                    {
                        num = hashSet.Count;
                        using Transaction transaction = new(doc);
                        transaction.Start("Purge unused");
                        doc.Delete(hashSet);
                        transaction.Commit();
                        continue;
                    }
                    break;
                }
            }
            catch { }
        }
    }
}