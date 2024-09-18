using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Transmit
{
    public class TransmitViewModel : ViewModelBase
    {
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
        public TransmitViewModel(EventHandlerTransmitModelsVMArg eventHandlerTransmitModelsUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerTransmitModelsUiArg;
            HelpMessage = HELP_MESSAGE;
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
    }
}