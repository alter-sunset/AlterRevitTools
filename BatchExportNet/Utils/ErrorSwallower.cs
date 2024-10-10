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
            _uiApp.DialogBoxShowing += TaskDialogBoxShowingEvent;
            _application.FailuresProcessing += Application_FailuresProcessing;
        }
        public void Dispose()
        {
            _uiApp.DialogBoxShowing -= TaskDialogBoxShowingEvent;
            _application.FailuresProcessing -= Application_FailuresProcessing;
        }
        private static void TaskDialogBoxShowingEvent(object sender, DialogBoxShowingEventArgs e)
        {
            if (e is TaskDialogShowingEventArgs dialogArgs)
            {
                string dialogId = dialogArgs.DialogId;
                int dialogResult = dialogId switch
                {
                    "TaskDialog_Missing_Third_Party_Updaters" => (int)TaskDialogResult.CommandLink1,
                    "TaskDialog_Missing_Third_Party_Updater" => (int)TaskDialogResult.CommandLink1,
                    _ => (int)TaskDialogResult.Close
                };

                dialogArgs.OverrideResult(dialogResult);
            }
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

                if (fseverity is FailureSeverity.Warning)
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