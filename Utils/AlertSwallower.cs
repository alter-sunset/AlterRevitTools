using Autodesk.Revit.DB;

namespace VLS.BatchExportNet.Utils
{
    static class AlertSwallower
    {
        public static void SwallowAlert(this Transaction transaction)
        {
            FailureHandlingOptions failOpt = transaction.GetFailureHandlingOptions();
            failOpt.SetFailuresPreprocessor(new CopyWatchAlertSwallower());
            transaction.SetFailureHandlingOptions(failOpt);
        }
    }
}