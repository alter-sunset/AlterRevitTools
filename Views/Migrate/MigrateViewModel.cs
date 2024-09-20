using System.Collections.Generic;
using System.Windows.Forms;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Migrate
{
    public class MigrateViewModel : ViewModelBase
    {
        public MigrateViewModel(EventHandlerMigrateModelsVMArg eventHandlerMigrateModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerMigrateModelsUiArg;
            Dictionary<HelpMessages, string> help = Help.GetHelpMessages();
            string _helpMessage = help.GetResultMessage(HelpMessages.Migrate);
            HelpMessage = _helpMessage;
        }

        private string _configPath;
        public string ConfigPath
        {
            get => _configPath;
            set
            {
                _configPath = value;
                OnPropertyChanged("ConfigPath");
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