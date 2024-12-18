using System.Windows.Forms;
using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.Migrate
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
        public override RelayCommand LoadListCommand =>
            _loadListCommand ??= new RelayCommand(obj => LoadList());
        private void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
            if (openFileDialog.ShowDialog() is DialogResult.OK)
                ConfigPath = openFileDialog.FileName;
        }
    }
}