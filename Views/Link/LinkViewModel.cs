using System.Windows;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.Link
{
    public class LinkViewModel(EventHandlerLinkModelsUiArg eventHandlerLinkModelsUiArg) : ViewModelBase
    {
        const string HELP_MESSAGE = "\tПлагин предназначен для пакетного добавления моделей в качестве Revit ссылок." +
                  "\n" +
                  "\tЕсли вы впервые используете плагин, и у вас нет ранее сохранённых списков, то вам необходимо выполнить следующее: " +
                  "используя кнопку \"Загрузить\" добавьте все модели объекта, которые необходимо передать. " +
                  "Если случайно были добавлены лишние файлы, выделите их и нажмите кнопку \"Удалить\"" +
                  "\n" +
                  "\tДалее укажите папку для сохранения. Прописать путь можно в ручную или же выбрать папку используя кнопку \"Обзор\"." +
                  "\n" +
                  "\tСохраните список кнопкой \"Сохранить список\" в формате (.txt)." +
                  "\n" +
                  "\tДалее этот список можно будет использовать для повторного добавления данного комплекта, используя кнопку \"Загрузить список\"." +
                  "\n\n" +
                  "\tЗапустите добавление кнопкой \"Запуск\".";

        private readonly EventHandlerLinkModelsUiArg _eventHandlerLinkModelsUiArg = eventHandlerLinkModelsUiArg;

        private bool _isCurrentWorkset = true;
        public bool IsCurrentWorkset
        {
            get => _isCurrentWorkset;
            set
            {
                _isCurrentWorkset = value;
                OnPropertyChanged("IsCurrentWorkset");
            }
        }
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

        private RelayCommand _raiseEventCommand;
        public override RelayCommand RaiseEventCommand
        {
            get
            {
                return _raiseEventCommand ??= new RelayCommand(obj =>
                {
                    _eventHandlerLinkModelsUiArg.Raise(this);
                });
            }
        }
    }
}
