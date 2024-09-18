using System.Windows.Forms;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.Detach
{
    public class DetachViewModel : ViewModelBase
    {
        private readonly EventHandlerDetachModelsUiArg _eventHandlerDetachModelsUiArg;
        const string HELP_MESSAGE = "\tПлагин предназначен для экспорта отсоединённых моделей." +
                "\n" +
                "\tЕсли вы впервые используете плагин, и у вас нет ранее сохранённых списков, то вам необходимо выполнить следующее: " +
                "используя кнопку \"Загрузить\" добавьте все модели объекта, которые необходимо передать. " +
                "Если случайно были добавлены лишние файлы, выделите их и нажмите кнопку \"Удалить\"" +
                "\n" +
                "\tДалее укажите папку для сохранения. Прописать путь можно в ручную или же выбрать папку используя кнопку \"Обзор\"." +
                "\n" +
                "\tВыберите режим экспорта:" +
                "\n" +
                "1. Все файлы будут помещены в одну папку." +
                "\n" +
                "2. Файлы будут помещены в соответствующие папки, то есть будет произведено обновление пути по маске." +
                "\n" +
                "\tСохраните список кнопкой \"Сохранить список\" в формате (.txt)." +
                "\n" +
                "\tДалее этот список можно будет использовать для повторного экспорта, используя кнопку \"Загрузить список\"." +
                "\n\n" +
                "\tЗапустите экспорт кнопкой \"Запуск\".";
        public DetachViewModel(EventHandlerDetachModelsUiArg eventHandlerDetachModelsUiArg)
        {
            _eventHandlerDetachModelsUiArg = eventHandlerDetachModelsUiArg;
            HelpMessage = HELP_MESSAGE;
        }

        private int _radioButtonMode = 0;
        public int RadionButtonMode
        {
            get => _radioButtonMode;
            set
            {
                _radioButtonMode = value;
                OnPropertyChanged("RadioButtonMode");
            }
        }

        private RelayCommand _radioButtonCommand;
        public override RelayCommand RadioButtonCommand
        {
            get
            {
                return _radioButtonCommand ??= new RelayCommand(RB_Command);
            }
        }
        private void RB_Command(object parameter)
        {
            switch ((string)parameter)
            {
                case "Folder":
                    _radioButtonMode = 1;
                    break;
                case "Mask":
                    _radioButtonMode = 2;
                    break;
            }
        }

        private string _maskIn = @"05_В_Работе\52_ПД";
        public string MaskIn
        {
            get => _maskIn;
            set
            {
                _maskIn = value;
                OnPropertyChanged("MaskIn");
            }
        }

        private string _maskOut = @"06_Общие\62_ПД";
        public string MaskOut
        {
            get => _maskOut;
            set
            {
                _maskOut = value;
                OnPropertyChanged("MaskOut");
            }
        }

        private RelayCommand _raiseEventCommand;
        public override RelayCommand RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ??= new RelayCommand(obj =>
                {
                    _eventHandlerDetachModelsUiArg.Raise(this);
                });
            }
        }
    }
}