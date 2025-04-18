using AlterTools.BatchExport.Utils;

namespace AlterTools.BatchExport.Views.Base
{
    public class ViewModelBaseExtended : ViewModelBase, IConfigBaseExtended
    {
        private string _namePrefix = string.Empty;
        public string NamePrefix
        {
            get => _namePrefix;
            set => SetProperty(ref _namePrefix, value);
        }

        private string _namePostfix = string.Empty;
        public string NamePostfix
        {
            get => _namePostfix;
            set => SetProperty(ref _namePostfix, value);
        }

        private string _worksetPrefix = string.Empty;

        protected string WorksetPrefix
        {
            get => _worksetPrefix;
            set => SetProperty(ref _worksetPrefix, value);
        }

        private bool _exportScopeView = true;
        public bool ExportScopeView
        {
            get => _exportScopeView;
            set
            {
                _exportScopeView = value;
                OnPropertyChanged(nameof(ExportScopeView));
                OnPropertyChanged(nameof(ExportScopeWhole));
            }
        }
        public bool ExportScopeWhole
        {
            get => !_exportScopeView;
            set
            {
                _exportScopeView = !value;
                OnPropertyChanged(nameof(ExportScopeWhole));
                OnPropertyChanged(nameof(ExportScopeView));
            }
        }

        public string[] WorksetPrefixes => _worksetPrefix.SplitBySemicolon();
    }
}