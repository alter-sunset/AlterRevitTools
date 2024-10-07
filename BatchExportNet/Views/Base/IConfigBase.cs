using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLS.BatchExportNet.Views.Base
{
    public interface IConfigBase
    {
        public List<string> Files { get; }
        public string ViewName { get; set; }
        public string FolderPath { get; set; }
    }
}
