using System.Linq;

namespace AlterTools.BatchExport.Views.Base
{
    public class ViewModelBase_Extended : ViewModelBase, IConfigBase_Extended
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

        public string[] WorksetPrefixes => _worksetPrefix.Split(';')
            .Select(e => e.Trim())
            .Distinct()
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToArray();
    }
}