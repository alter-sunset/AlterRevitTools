using System.Collections.Generic;

namespace VLS.BatchExportNet.Views.Base
{
    public interface IConfigBase
    {
        public List<string> Files { get; }
        public string ViewName { get; set; }
        public string FolderPath { get; set; }
    }
}