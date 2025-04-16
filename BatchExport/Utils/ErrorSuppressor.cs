﻿using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;

namespace AlterTools.BatchExport.Utils
{
    public class ErrorSuppressor : IDisposable
    {
        private readonly UIApplication _uiApp;
        private readonly Application _app;

        public ErrorSuppressor(UIApplication uiApp)
        {
            _uiApp = uiApp;
            _app = uiApp.Application;

            _uiApp.DialogBoxShowing += TaskDialogBoxShowingEvent;
            _app.FailuresProcessing += ApplicationFailuresProcessing;
        }

        private static void TaskDialogBoxShowingEvent(object sender, DialogBoxShowingEventArgs e)
        {
            if (e is not TaskDialogShowingEventArgs dialogArgs) return;

            int dialogResult = dialogArgs.DialogId.StartsWith("TaskDialog_Missing_Third_Party_Updater")
                                ? (int)TaskDialogResult.CommandLink1
                                : (int)TaskDialogResult.Close;

            dialogArgs.OverrideResult(dialogResult);
        }

        private static void ApplicationFailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor a = e.GetFailuresAccessor();
            FailureProcessingResult result = PreprocessFailures(a);
            e.SetProcessingResult(result);
        }

        private static FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures = a.GetFailureMessages();

            foreach (FailureMessageAccessor f in failures)
            {
                FailureSeverity fseverity = a.GetSeverity();

                if (FailureSeverity.Warning == fseverity)
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

        public void Dispose()
        {
            _uiApp.DialogBoxShowing -= TaskDialogBoxShowingEvent;
            _app.FailuresProcessing -= ApplicationFailuresProcessing;
        }
    }
}