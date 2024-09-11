using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace VLS.BatchExportNet.Utils
{
    public class CopyWatchAlertSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            IList<FailureMessageAccessor> failures = a.GetFailureMessages();
            foreach (FailureMessageAccessor f in failures)
            {
                FailureDefinitionId id = f.GetFailureDefinitionId();

                if (BuiltInFailures.CopyMonitorFailures.CopyWatchAlert == id)
                {
                    a.DeleteWarning(f);
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}