using System.Windows.Forms;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Migrate
{
    public class MigrateViewModel : ViewModelBase
    {
        public MigrateViewModel(EventHandlerMigrateModelsVMArg eventHandlerMigrateModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerMigrateModelsUiArg;
            HelpMessage =
                Help.GetHelpDictionary().
                GetResultMessage(HelpMessageType.Migrate);
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
                    OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
                    DialogResult result = openFileDialog.ShowDialog();
                    if (result == DialogResult.OK)
                        ConfigPath = openFileDialog.FileName;
                });
            }
        }
    }
}