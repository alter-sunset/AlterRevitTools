using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace VLS.BatchExportNet.Utils
{
    public class CopyWatchAlertSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            IEnumerable<FailureMessageAccessor> failures = a.GetFailureMessages()
                .Where(f => f.GetFailureDefinitionId() ==
                    BuiltInFailures.CopyMonitorFailures.CopyWatchAlert);
            foreach (FailureMessageAccessor f in failures)
            {
                a.DeleteWarning(f);
            }
            return FailureProcessingResult.Continue;
        }
    }
}