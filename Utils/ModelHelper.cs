using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using System.IO;
using System.Security.Cryptography;
using System;

namespace VLS.BatchExportNet.Utils
{
    public static class ModelHelper
    {
        public static WorksetConfiguration CloseWorksetsWithLinks(ModelPath modelPath)
        {
            WorksetConfiguration worksetConfiguration = new(WorksetConfigurationOption.OpenAllWorksets);

            IList<WorksetPreview> worksets = WorksharingUtils.GetUserWorksetInfo(modelPath);
            IList<WorksetId> worksetIds = new List<WorksetId>();

            foreach (WorksetPreview worksetPreview in worksets)
            {
                if (worksetPreview.Name.StartsWith("99") || worksetPreview.Name.StartsWith("00"))
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

        public static void FreeTheModel(Document document)
        {
            RelinquishOptions relinquishOptions = new(true);
            TransactWithCentralOptions transactWithCentralOptions = new();
            WorksharingUtils.RelinquishOwnership(document, relinquishOptions, transactWithCentralOptions);
        }
        public static string MD5Hash(string fileName)
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