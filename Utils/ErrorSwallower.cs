using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;

namespace VLS.BatchExportNet.Utils
{
    public class ErrorSwallower : IDisposable
    {
        private readonly UIApplication _uiApp;
        private readonly Application _application;
        public ErrorSwallower(UIApplication uiApp, Application application)
        {
            _uiApp = uiApp;
            _application = application;
            _uiApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(TaskDialogBoxShowingEvent);
            _application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(Application_FailuresProcessing);
        }
        public void Dispose()
        {
            _uiApp.DialogBoxShowing -= new EventHandler<DialogBoxShowingEventArgs>(TaskDialogBoxShowingEvent);
            _application.FailuresProcessing -= new EventHandler<FailuresProcessingEventArgs>(Application_FailuresProcessing);
        }
        private static void TaskDialogBoxShowingEvent(object sender, DialogBoxShowingEventArgs e)
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
        private static void Application_FailuresProcessing(object sender, FailuresProcessingEventArgs e)
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
                    a.DeleteWarning(f);
                else
                {
                    a.ResolveFailure(f);
                    return FailureProcessingResult.ProceedWithCommit;
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}