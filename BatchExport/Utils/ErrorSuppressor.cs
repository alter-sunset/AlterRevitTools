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

        private static void TaskDialogBoxShowingEvent(object sender, DialogBoxShowingEventArgs args)
        {
            if (args is not TaskDialogShowingEventArgs dialogArgs) return;

            int dialogResult = dialogArgs.DialogId.StartsWith("TaskDialog_Missing_Third_Party_Updater")
                                ? (int)TaskDialogResult.CommandLink1
                                : (int)TaskDialogResult.Close;

            dialogArgs.OverrideResult(dialogResult);
        }

        private static void ApplicationFailuresProcessing(object sender, FailuresProcessingEventArgs args)
        {
            FailuresAccessor accessor = args.GetFailuresAccessor();
            FailureProcessingResult result = PreprocessFailures(accessor);
            args.SetProcessingResult(result);
        }

        private static FailureProcessingResult PreprocessFailures(FailuresAccessor accessor)
        {
            IList<FailureMessageAccessor> failures = accessor.GetFailureMessages();

            foreach (FailureMessageAccessor failure in failures)
            {
                FailureSeverity fSeverity = accessor.GetSeverity();

                if (FailureSeverity.Warning == fSeverity)
                {
                    accessor.DeleteWarning(failure);
                }
                else
                {
                    accessor.ResolveFailure(failure);
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