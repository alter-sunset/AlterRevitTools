using Autodesk.Revit.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Controls;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Collections.ObjectModel;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Link
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class LinkModelsUi : Window
    {
        public ObservableCollection<ListBoxItem> listBoxItems = new();
        private readonly EventHandlerLinkModelsUiArg _eventHandlerLinkModelsUiArg;
        public LinkModelsUi(UIApplication uiApp, EventHandlerLinkModelsUiArg eventHandlerLinkModelsUiArg)
        {
            InitializeComponent();
            this.DataContext = listBoxItems;
            _eventHandlerLinkModelsUiArg = eventHandlerLinkModelsUiArg;
        }
        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                DefaultExt = ".rvt",
                Filter = "Revit Files (.rvt)|*.rvt"
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    ListBoxItem listBoxItem = new ListBoxItem() { Content = file, Background = Brushes.White };
                    if (!listBoxItems.Any(cont => cont.Content.ToString() == file))
                        listBoxItems.Add(listBoxItem);
                }
            }
        }
        private void ButtonLoadList_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                DefaultExt = ".txt",
                Filter = "Текстовый файл (.txt)|*.txt"
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                listBoxItems.Clear();

                IEnumerable listRVTFiles = File.ReadLines(openFileDialog.FileName);

                //int count = listBoxItems.Count;

                foreach (string rVTFile in listRVTFiles)
                {
                    ListBoxItem listBoxItem = new ListBoxItem() { Content = rVTFile, Background = Brushes.White };
                    if (!listBoxItems.Any(cont => cont.Content.ToString() == rVTFile) && rVTFile.EndsWith(".rvt"))
                    {
                        listBoxItems.Add(listBoxItem);
                    }
                }

                if (listBoxItems.Count.Equals(0))
                {
                    System.Windows.MessageBox.Show("В текстовом файле не было найдено подходящей информации");
                }
            }
        }
        private void ButtonSaveList_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "ListOfRVTFilesToLink",
                DefaultExt = ".txt",
                Filter = "Текстовый файл (.txt)|*.txt"
            };

            DialogResult result = saveFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                File.Delete(fileName);

                foreach (string fileRVT in listBoxItems.Select(cont => cont.Content.ToString()))
                {
                    if (!File.Exists(fileName))
                    {
                        File.WriteAllText(fileName, fileRVT);
                    }
                    else
                    {
                        string toWrite = "\n" + fileRVT;
                        File.AppendAllText(fileName, toWrite);
                    }
                }
            }
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            List<ListBoxItem> itemsToRemove = ListBoxRVTFiles.SelectedItems.Cast<ListBoxItem>().ToList();
            if (itemsToRemove.Count != 0)
            {
                foreach (ListBoxItem item in itemsToRemove)
                {
                    listBoxItems.Remove(item);
                }
            }
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e) => _eventHandlerLinkModelsUiArg.Raise(this);
        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
        private void ButtonErase_Click(object sender, RoutedEventArgs e)
        {
            listBoxItems.Clear();
        }
        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            const string msg = "\tПлагин предназначен для пакетного добавления моделей в качестве Revit ссылок." +
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
                  "\tЗапустите добавление кнопкой \"ОК\".";
            MessageBox.Show(msg, "Справка");
        }
    }
}