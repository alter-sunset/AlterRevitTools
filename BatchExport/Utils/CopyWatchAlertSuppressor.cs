using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Utils
{
    public class CopyWatchAlertSuppressor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor accessor)
        {
            List<FailureMessageAccessor> failures = accessor.GetFailureMessages()
                .Where(failure =>
                    BuiltInFailures.CopyMonitorFailures.CopyWatchAlert ==
                    failure.GetFailureDefinitionId())
                .ToList();

            failures.ForEach(accessor.DeleteWarning);

            return FailureProcessingResult.Continue;
        }
    }
}