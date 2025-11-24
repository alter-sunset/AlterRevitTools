using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.BatchExport.Utils;

public class ErrorSuppressor : IDisposable
{
    private readonly Application _app;
    private readonly UIApplication _uiApp;
    private readonly DwgImportDialogSuppressor _dwgImportDialogSuppressor;

    public ErrorSuppressor(UIApplication uiApp)
    {
        _uiApp = uiApp;
        _app = uiApp.Application;
        _dwgImportDialogSuppressor = new DwgImportDialogSuppressor();

        _uiApp.DialogBoxShowing += TaskDialogBoxShowingEvent;
        _app.FailuresProcessing += ApplicationFailuresProcessing;
    }

    public void Dispose()
    {
        _uiApp.DialogBoxShowing -= TaskDialogBoxShowingEvent;
        _app.FailuresProcessing -= ApplicationFailuresProcessing;
        _dwgImportDialogSuppressor.Dispose();
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
        using FailuresAccessor accessor = args.GetFailuresAccessor();
        FailureProcessingResult result = PreprocessFailures(accessor);
        args.SetProcessingResult(result);
    }

    private static FailureProcessingResult PreprocessFailures(FailuresAccessor accessor)
    {
        IList<FailureMessageAccessor> failures = accessor.GetFailureMessages();

        foreach (FailureMessageAccessor failure in failures)
        {
            FailureSeverity fSeverity = accessor.GetSeverity();

            if (fSeverity is not FailureSeverity.Warning)
            {
                accessor.ResolveFailure(failure);
                return FailureProcessingResult.ProceedWithCommit;
            }
            accessor.DeleteWarning(failure);
        }

        return FailureProcessingResult.Continue;
    }
}