using Autodesk.Revit.DB;
using System.Linq;
using System.Collections.Generic;

namespace AlterTools.BatchExport.Utils
{
    public class CopyWatchAlertSuppressor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor accessor)
        {
            List<FailureMessageAccessor> failures = accessor.GetFailureMessages()
                                                     .Where(failure => BuiltInFailures.CopyMonitorFailures.CopyWatchAlert == failure.GetFailureDefinitionId())
                                                     .ToList();

            failures.ForEach(accessor.DeleteWarning);

            return FailureProcessingResult.Continue;
        }
    }
}