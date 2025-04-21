using System.Windows.Forms;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Migrate
{
    public class MigrateViewModel : ViewModelBase
    {
        private string _configPath;

        public MigrateViewModel(EventHandlerMigrate eventHandlerMigrate)
        {
            EventHandlerBase = eventHandlerMigrate;
            HelpMessage = Help.GetHelpDictionary()
                .GetResultMessage(HelpMessageType.Migrate);
        }

        public string ConfigPath
        {
            get => _configPath;
            set => SetProperty(ref _configPath, value);
        }

        protected override void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                ConfigPath = openFileDialog.FileName;
            }
        }
    }
}