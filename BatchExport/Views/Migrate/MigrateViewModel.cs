using System.Windows.Forms;
using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Migrate
{
    public class MigrateViewModel : ViewModelBase
    {
        public MigrateViewModel(EventHandlerMigrate eventHandlerMigrate)
        {
            EventHandlerBase = eventHandlerMigrate;
            HelpMessage =
                Help.GetHelpDictionary().
                    GetResultMessage(HelpMessageType.Migrate);
        }

        private string _configPath;
        public string ConfigPath
        {
            get => _configPath;
            set => SetProperty(ref _configPath, value);
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand
        {
            get
            {
                if (_loadListCommand is null)
                    _loadListCommand = new RelayCommand(obj => LoadList());
                return _loadListCommand;
            }
        }
        private void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
            if (openFileDialog.ShowDialog() is DialogResult.OK)
                ConfigPath = openFileDialog.FileName;
        }
    }
}