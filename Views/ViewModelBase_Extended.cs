using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLS.BatchExportNet.Views
{
    public class ViewModelBase_Extended : ViewModelBase
    {
        private string _namePrefix = "";
        public string NamePrefix
        {
            get => _namePrefix;
            set
            {
                _namePrefix = value;
                OnPropertyChanged("NamePrefix");
            }
        }

        private string _namePostfix = "";
        public string NamePostfix
        {
            get => _namePostfix;
            set
            {
                _namePostfix = value;
                OnPropertyChanged("NamePostfix");
            }
        }

        private string _worksetPrefix = "";
        public string WorksetPrefix
        {
            get => _worksetPrefix;
            set
            {
                _worksetPrefix = value;
                OnPropertyChanged("WorksetPrefix");
            }
        }

        private string _viewName = "Navisworks";
        public string ViewName
        {
            get => _viewName;
            set
            {
                _viewName = value;
                OnPropertyChanged("ViewName");
            }
        }

        private bool _exportScopeView = true;
        public bool ExportScopeView
        {
            get => _exportScopeView;
            set
            {
                _exportScopeView = value;
                OnPropertyChanged("ExportScopeView");
            }
        }
        public bool ExportScopeWhole
        {
            get => !_exportScopeView;
            set
            {
                _exportScopeView = !value;
                OnPropertyChanged("ExportScopeWhole");
            }
        }
    }
}
