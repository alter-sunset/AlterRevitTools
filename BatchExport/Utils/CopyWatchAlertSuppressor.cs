using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Utils;

public class CopyWatchAlertSuppressor : IFailuresPreprocessor
{
    public FailureProcessingResult PreprocessFailures(FailuresAccessor accessor)
    {
        List<FailureMessageAccessor> failures = accessor.GetFailureMessages()
            .Where(failure => failure.GetFailureDefinitionId() == BuiltInFailures.CopyMonitorFailures.CopyWatchAlert)
            .ToList();

        failures.ForEach(accessor.DeleteWarning);

        return FailureProcessingResult.Continue;
    }
}