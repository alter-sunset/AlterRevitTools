using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.IFC
{
    /// <summary>
    /// Interaction logic for NWCExportUi.xaml
    /// </summary>
    public partial class IFCExportUi : Window
    {
        public ObservableCollection<ListBoxItem> listBoxItems = new ObservableCollection<ListBoxItem>();

        private readonly EventHandlerIFCExportUiArg _eventHandlerIFCExportUiArg;

        public static readonly Dictionary<int, IFCVersion> indexToIFCVersion = new Dictionary<int, IFCVersion>()
        {
            {0, IFCVersion.Default },
            {1, IFCVersion.IFCBCA },
            {2, IFCVersion.IFC2x2 },
            {3, IFCVersion.IFC2x3 },
            {4, IFCVersion.IFCCOBIE },
            {5, IFCVersion.IFC2x3CV2 },
            {6, IFCVersion.IFC4 },
            {7, IFCVersion.IFC2x3FM },
            {8, IFCVersion.IFC4RV },
            {9, IFCVersion.IFC4DTV },
            {10, IFCVersion.IFC2x3BFM }
        };

        public IFCExportUi(UIApplication uiApp, EventHandlerIFCExportUiArg eventHandlerIFCExportUiArg)
        {
            InitializeComponent();
            DataContext = listBoxItems;
            _eventHandlerIFCExportUiArg = eventHandlerIFCExportUiArg;
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
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
                Multiselect = false,
                DefaultExt = ".json",
                Filter = "Файл JSON (.json)|*.json"
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                using FileStream file = File.OpenRead(openFileDialog.FileName);
                try
                {
                    IFCForm form = JsonSerializer.Deserialize<IFCForm>(file);
                    IFCFormDeserilaizer(form);
                    form.Dispose();
                }
                catch
                {
                    MessageBox.Show("Неверная схема файла");
                }
            }
        }
        public void IFCFormDeserilaizer(IFCForm form)
        {
            TextBoxFolder.Text = form.DestinationFolder;
            TextBoxPrefix.Text = form.Prefix;
            TextBoxPostfix.Text = form.Postfix;
            TextBoxMapping.Text = form.FamilyMappingFile;
            CheckBoxExportBaseQuantities.IsChecked = form.ExportBaseQuantities;

            ComboBoxIFCVersion.SelectedIndex = indexToIFCVersion
                .FirstOrDefault(e => e.Value == form.FileVersion)
                .Key;

            CheckBoxWallAndColumnSplitting.IsChecked = form.WallAndColumnSplitting;

            switch (form.ExportView)
            {
                case true:
                    RadioButtonExportScopeView.IsChecked = true;
                    break;
                case false:
                    RadioButtonExportScopeModel.IsChecked = true;
                    break;
            }

            TextBoxExportScopeViewName.Text = form.ViewName;

            ComboBoxSpaceBoundaryLevel.SelectedIndex = form.SpaceBoundaryLevel;

            listBoxItems.Clear();
            foreach (string file in form.RVTFiles)
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                ListBoxItem listBoxItem = new ListBoxItem() { Content = file, Background = Brushes.White };
                if (!listBoxItems.Any(cont => cont.Content.ToString() == file)
                    || file.EndsWith(".rvt", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    listBoxItems.Add(listBoxItem);
                }
            }
        }
        private void ButtonSaveList_Click(object sender, RoutedEventArgs e)
        {
            IFCForm form = IFCFormSerializer();

            SaveFileDialog saveFileDialog = new()
            {
                FileName = "ConfigBatchExportIFC",
                DefaultExt = ".json",
                Filter = "Файл JSON (.json)|*.json"
            };

            DialogResult result = saveFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                File.Delete(fileName);

                File.WriteAllText(fileName, JsonSerializer.Serialize(form));
            }

            form.Dispose();
        }

        private IFCForm IFCFormSerializer()
        {
            IFCForm form = new()
            {
                ExportBaseQuantities = (bool)CheckBoxExportBaseQuantities.IsChecked,
                FamilyMappingFile = TextBoxMapping.Text,

                FileVersion = indexToIFCVersion
                    .FirstOrDefault(e => e.Key == ComboBoxIFCVersion.SelectedIndex)
                    .Value,

                SpaceBoundaryLevel = ComboBoxSpaceBoundaryLevel.SelectedIndex,
                WallAndColumnSplitting = (bool)CheckBoxWallAndColumnSplitting.IsChecked,
                DestinationFolder = TextBoxFolder.Text,
                Prefix = TextBoxPrefix.Text,
                Postfix = TextBoxPostfix.Text,

                RVTFiles = listBoxItems
                .Select(cont => cont.Content.ToString())
                .ToList(),

                ViewName = TextBoxExportScopeViewName.Text,
                ExportView = (bool)RadioButtonExportScopeView.IsChecked
            };

            return form;
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
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() { SelectedPath = TextBoxFolder.Text };
            DialogResult result = folderBrowserDialog.ShowDialog();
            string folderPath = folderBrowserDialog.SelectedPath;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxFolder.Text = folderPath;
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            _eventHandlerIFCExportUiArg.Raise(this);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonErase_Click(object sender, RoutedEventArgs e)
        {
            listBoxItems.Clear();
            TextBoxFolder.Clear();
        }
        private void TextBoxFolder_TextChanged(object sender, TextChangedEventArgs e) { }

        private void TextBoxMapping_TextChanged(object sender, TextChangedEventArgs e) { }

        private void ButtonBrowseMapping_Click(object sender, RoutedEventArgs e) { }

        private void RadioButtonExportScopeView_Checked(object sender, RoutedEventArgs e) { }

        private void RadioButtonExportScopeModel_Checked(object sender, RoutedEventArgs e) { }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            const string msg = "\tПлагин предназначен для пакетного экспорта файлов в формат IFC." +
                 "\n" +
                 "\tЕсли вы впервые используете плагин, и у вас нет ранее сохранённых файлов конфигурации, то вам необходимо выполнить следующее: " +
                 "используя кнопку \"Загрузить\" добавьте все модели объекта, которые необходимо экспортировать. " +
                 "Если случайно были добавлены лишние файлы, выделите их и нажмите кнопку \"Удалить\"" +
                 "\n" +
                 "\tДалее укажите папку для сохранения. Прописать путь можно в ручную или же выбрать папку используя кнопку \"Обзор\"." +
                 "\n" +
                 "\tЗадайте префикс и постфикс, которые будет необходимо добавить в название файлов. Если такой необходимости нет, то оставьте поля пустыми." +
                 "\n" +
                 "\tВыберите необходимые свойства экспорта. По умолчанию стоят стандартные настройки, с которыми чаще всего работают." +
                 "\n" +
                 "\tСохраните конфигурацию кнопкой \"Сохранить список\" в формате (.JSON)." +
                 "\n" +
                 "\tДалее эту конфигурацию можно будет использовать для повторного экспорта, используя кнопку \"Загрузить список\"." +
                 "\n\n" +
                 "\tЗапустите экспорт кнопкой \"ОК\".";
            MessageBox.Show(msg, "Справка");
        }
    }
}