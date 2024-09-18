using System.Windows.Forms;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.Migrate
{
    public class MigrateViewModel : ViewModelBase
    {
        private readonly EventHandlerMigrateModelsUiArg _eventHandlerMigrateModelsUiArg;
        private const string HELP_MESSAGE = "\tПлагин предназначен для миграции проекта в новое место с сохранением структуры связей, как внутри папок, так и внутри самих моделей." +
                "\n" +
                "\tОткройте или вставьте ссылку на Json конфиг, который хранит в себе структуру типа Dictionary<string, string>," +
                "\n" +
                "где первый string - текущий путь к файлу, второй - новый путь." +
                "\n" +
                "Пример:" +
                "\n" +
                "{ \"C:\\oldfile.rvt\": \"C:\\newfile.rvt\",}";
        public MigrateViewModel(EventHandlerMigrateModelsUiArg eventHandlerMigrateModelsUiArg)
        {
            _eventHandlerMigrateModelsUiArg = eventHandlerMigrateModelsUiArg;
            HelpMessage = HELP_MESSAGE;
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

        private RelayCommand _raiseEventCommand;
        public override RelayCommand RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ??= new RelayCommand(obj =>
                {
                    _eventHandlerMigrateModelsUiArg.Raise(this);
                });
            }
        }
    }
}