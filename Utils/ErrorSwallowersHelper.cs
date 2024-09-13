using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace VLS.BatchExportNet.Utils
{
    public static class ErrorSwallowersHelper
    {
        public static void TaskDialogBoxShowingEvent(object sender, DialogBoxShowingEventArgs e)
        {
            TaskDialogShowingEventArgs e2 = e as TaskDialogShowingEventArgs;

            string dialogId = e2.DialogId;
            int dialogResult;
            bool isConfirm;

            switch (dialogId)
            {
                case "TaskDialog_Missing_Third_Party_Updaters":
                case "TaskDialog_Missing_Third_Party_Updater":
                    isConfirm = true;
                    dialogResult = (int)TaskDialogResult.CommandLink1;
                    break;
                default:
                    isConfirm = true;
                    dialogResult = (int)TaskDialogResult.Close;
                    break;
            }

            if (isConfirm)
                e2.OverrideResult(dialogResult);
        }
        public static void Application_FailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor = e.GetFailuresAccessor();
            FailureProcessingResult response = PreprocessFailures(failuresAccessor);
            e.SetProcessingResult(response);
        }
        private static FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures = a.GetFailureMessages();

            foreach (FailureMessageAccessor f in failures)
            {
                FailureSeverity fseverity = a.GetSeverity();

                if (fseverity == FailureSeverity.Warning)
                {
                    a.DeleteWarning(f);
                }
                else
                {
                    a.ResolveFailure(f);
                    return FailureProcessingResult.ProceedWithCommit;
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
    public class CopyWatchAlertSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures = a.GetFailureMessages();
            foreach (FailureMessageAccessor f in failures)
            {
                FailureDefinitionId id = f.GetFailureDefinitionId();

                if (BuiltInFailures.CopyMonitorFailures.CopyWatchAlert == id)
                {
                    a.DeleteWarning(f);
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}