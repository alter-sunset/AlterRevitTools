using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Autodesk.Revit.DB;
using Transaction = Autodesk.Revit.DB.Transaction;
using VLS.BatchExportNet.Views;
using Autodesk.Revit.UI;

namespace VLS.BatchExportNet.Utils
{
    static class ModelHelper
    {
        internal static WorksetConfiguration CloseWorksetsWithLinks(ModelPath modelPath, params string[] prefixes)
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
        internal static bool IsViewEmpty(Document doc, Element element)
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
        internal static void FreeTheModel(Document doc)
        {
            RelinquishOptions relinquishOptions = new(true);
            TransactWithCentralOptions transactWithCentralOptions = new();
            WorksharingUtils.RelinquishOwnership(doc, relinquishOptions, transactWithCentralOptions);
        }
        internal static void DeleteAllLinks(Document doc)
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
        internal static void Finisher(ViewModelBase viewModel, string id, string msg = "Задание выполнено")
        {
            TaskDialog taskDialog = new("Готово!")
            {
                CommonButtons = TaskDialogCommonButtons.Close,
                Id = id,
                MainContent = msg
            };
            viewModel.IsViewEnabled = false;
            taskDialog.Show();
            viewModel.IsViewEnabled = true;
        }
    }
}