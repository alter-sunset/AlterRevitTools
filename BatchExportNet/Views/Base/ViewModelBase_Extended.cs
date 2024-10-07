namespace VLS.BatchExportNet.Views.Base
{
    public class ViewModelBase_Extended : ViewModelBase, IConfigBase_Extended
    {
        private string _namePrefix = "";
        public string NamePrefix
        {
            get => _namePrefix;
            set
            {
                _namePrefix = value.Trim();
                OnPropertyChanged(nameof(NamePrefix));
            }
        }

        private string _namePostfix = "";
        public string NamePostfix
        {
            get => _namePostfix;
            set
            {
                _namePostfix = value.Trim();
                OnPropertyChanged(nameof(NamePostfix));
            }
        }

        private string _worksetPrefix = "";
        public string WorksetPrefix
        {
            get => _worksetPrefix;
            set
            {
                _worksetPrefix = value.Trim();
                OnPropertyChanged(nameof(WorksetPrefix));
            }
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

        public string[] WorksetPrefixes => _worksetPrefix.Split(';');
    }
}