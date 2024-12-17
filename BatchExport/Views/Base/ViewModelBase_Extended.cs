namespace VLS.BatchExport.Views.Base
{
    public class ViewModelBase_Extended : ViewModelBase, IConfigBase_Extended
    {
        private string _namePrefix = "";
        public string NamePrefix
        {
            get => _namePrefix;
            set => SetProperty(ref _namePrefix, value);
        }

        private string _namePostfix = "";
        public string NamePostfix
        {
            get => _namePostfix;
            set => SetProperty(ref _namePostfix, value);
        }

        private string _worksetPrefix = "";
        public string WorksetPrefix
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

        public string[] WorksetPrefixes => _worksetPrefix.Split(';');
    }
}