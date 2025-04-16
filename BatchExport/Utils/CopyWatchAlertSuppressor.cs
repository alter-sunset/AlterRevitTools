using Autodesk.Revit.DB;
using System.Linq;
using System.Collections.Generic;

namespace AlterTools.BatchExport.Utils
{
    public class CopyWatchAlertSuppressor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            List<FailureMessageAccessor> failures = a.GetFailureMessages()
                .Where(f => f.GetFailureDefinitionId() == BuiltInFailures.CopyMonitorFailures.CopyWatchAlert)
                .ToList();

            failures.ForEach(a.DeleteWarning);

            return FailureProcessingResult.Continue;
        }
    }
}