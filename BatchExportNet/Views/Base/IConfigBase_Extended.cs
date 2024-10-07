using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLS.BatchExportNet.Views.Base
{
    public interface IConfigBase_Extended : IConfigBase
    {
        public string NamePrefix { get; set; }
        public string NamePostfix { get; set; }
        public string[] WorksetPrefixes { get; }
        public bool ExportScopeView { get; }
        public bool ExportScopeWhole { get; set; }
    }
}
