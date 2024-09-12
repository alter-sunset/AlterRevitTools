using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;
using RadioButton = System.Windows.Controls.RadioButton;
using Autodesk.Revit.UI;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Detach
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class DetachModelsUi : Window
    {
        public ObservableCollection<ListBoxItem> listBoxItems = new();

        private readonly EventHandlerDetachModelsUiArg _eventHandlerDetachModelsUiArg;

        public int RadioButtonSavingPathMode = 0;

        public DetachModelsUi(UIApplication uiApp, EventHandlerDetachModelsUiArg eventHandlerDetachModelsUiArg)
        {
            InitializeComponent();
            this.DataContext = listBoxItems;

            _eventHandlerDetachModelsUiArg = eventHandlerDetachModelsUiArg;
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
                    ListBoxItem listBoxItem = new() { Content = file, Background = Brushes.White };
                    if (!listBoxItems.Any(cont => cont.Content.ToString() == file))
                    {
                        listBoxItems.Add(listBoxItem);
                    }
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
                TextBoxFolder.Text = Path.GetDirectoryName(openFileDialog.FileName);
            }
        }
        private void ButtonSaveList_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                FileName = "ListOfRVTFilesToNWC",
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
                TextBoxFolder.Text = Path.GetDirectoryName(saveFileDialog.FileName);
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

        private void ButtonBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new() { SelectedPath = TextBoxFolder.Text };
            DialogResult result = folderBrowserDialog.ShowDialog();
            string folderPath = folderBrowserDialog.SelectedPath;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxFolder.Text = folderPath;
            }
        }
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            _eventHandlerDetachModelsUiArg.Raise(this);
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (sender as RadioButton);

            string selection = rb.Tag.ToString();

            switch (selection)
            {
                case "Folder":
                    RadioButtonSavingPathMode = 1;
                    break;
                case "Mask":
                    RadioButtonSavingPathMode = 3;
                    break;
            }
        }
        private void ButtonErase_Click(object sender, RoutedEventArgs e)
        {
            listBoxItems.Clear();
            TextBoxFolder.Clear();
        }
        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            const string msg = "\tПлагин предназначен для экспорта отсоединённых моделей." +
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
                "\tЗапустите экспорт кнопкой \"ОК\".";
            MessageBox.Show(msg, "Справка");
        }
    }
}