using System.Windows;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.Transmit
{
    public class TransmitViewModel(EventHandlerTransmitModelsUiArg eventHandlerTransmitModelsUiArg) : ViewModelBase
    {
        private readonly EventHandlerTransmitModelsUiArg _eventHandlerTransmitModelsUiArg = eventHandlerTransmitModelsUiArg;
        private const string HELP_MESSAGE = "\tПлагин предназначен для пакетной передачи моделей и реализует схожий функционал с плагином \"eTransmit\"." +
                  "\n" +
                  "\tЕсли вы впервые используете плагин, и у вас нет ранее сохранённых списков, то вам необходимо выполнить следующее: " +
                  "используя кнопку \"Загрузить\" добавьте все модели объекта, которые необходимо передать. " +
                  "Если случайно были добавлены лишние файлы, выделите их и нажмите кнопку \"Удалить\"" +
                  "\n" +
                  "\tДалее укажите папку для сохранения. Прописать путь можно в ручную или же выбрать папку используя кнопку \"Обзор\"." +
                  "\n" +
                  "\tСохраните список кнопкой \"Сохранить список\" в формате (.txt)." +
                  "\n" +
                  "\tДалее этот список можно будет использовать для повторного экспорта, используя кнопку \"Загрузить список\"." +
                  "\n\n" +
                  "\tЗапустите экспорт кнопкой \"Запуск\".";
        private RelayCommand _helpCommand;
        public override RelayCommand HelpCommand
        {
            get
            {
                return _helpCommand ??= new RelayCommand(obj =>
                {
                    MessageBox.Show(HELP_MESSAGE, "Справка");
                });
            }
        }

        private bool _isSameFolder;
        public bool IsSameFolder
        {
            get => _isSameFolder;
            set
            {
                _isSameFolder = value;
                OnPropertyChanged("IsSameFolder");
            }
        }
        private RelayCommand _raiseEventCommand;
        public override RelayCommand RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ??= new RelayCommand(obj =>
                {
                    _eventHandlerTransmitModelsUiArg.Raise(this);
                });
            }
        }

    }
}
