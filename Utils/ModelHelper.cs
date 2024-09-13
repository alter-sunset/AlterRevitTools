using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Autodesk.Revit.DB;

namespace VLS.BatchExportNet.Utils
{
    static class ModelHelper
    {
        internal static WorksetConfiguration CloseWorksetsWithLinks(ModelPath modelPath, params string[] prefixes)
        {
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);

            IList<WorksetPreview> worksets = WorksharingUtils.GetUserWorksetInfo(modelPath);
            IList<WorksetId> worksetIds = new List<WorksetId>();

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
        internal static bool IsViewEmpty(Document document, Element element)
        {
            View3D view = element as View3D;
            try
            {
                using FilteredElementCollector collector = new(document, view.Id);
                return !collector.Where(e => e.Category != null && e.GetType() != typeof(RevitLinkInstance)).Any(e => e.CanBeHidden(view));
            }
            catch
            {
                return true;
            }
        }
        internal static void FreeTheModel(Document document)
        {
            RelinquishOptions relinquishOptions = new(true);
            TransactWithCentralOptions transactWithCentralOptions = new();
            WorksharingUtils.RelinquishOwnership(document, relinquishOptions, transactWithCentralOptions);
        }
        internal static string MD5Hash(string fileName)
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
    }
}