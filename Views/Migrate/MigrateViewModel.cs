using System.Windows.Forms;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Migrate
{
    public class MigrateViewModel : ViewModelBase
    {
        public MigrateViewModel(EventHandlerMigrateModelsVMArg eventHandlerMigrateModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerMigrateModelsUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessages.Migrate);
        }

        private string _configPath;
        public string ConfigPath
        {
            get => _configPath;
            set
            {
                _configPath = value.Trim();
                OnPropertyChanged(nameof(ConfigPath));
            }
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand
        {
            get
            {
                return _loadListCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Multiselect = false,
                        DefaultExt = ".json",
                        Filter = "Файл JSON (.json)|*.json"
                    };

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        ConfigPath = openFileDialog.FileName;
                    }
                });
            }
        }
    }
}