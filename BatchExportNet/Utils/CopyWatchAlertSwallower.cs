﻿using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace VLS.BatchExportNet.Utils
{
    public class CopyWatchAlertSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            List<FailureMessageAccessor> failures = a.GetFailureMessages()
                .Where(f => f.GetFailureDefinitionId() ==
                    BuiltInFailures.CopyMonitorFailures.CopyWatchAlert)
                .ToList();

            failures.ForEach(a.DeleteWarning);
            return FailureProcessingResult.Continue;
        }
    }
}