using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using VLS.BatchExportNet.Source;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace VLS.BatchExportNet.Views.NWC
{
    /// <summary>
    /// Interaction logic for NWCExportUi.xaml
    /// </summary>
    public partial class NWCExportUi : Window
    {
        public ObservableCollection<ListBoxItem> listBoxItems = [];
        private List<string> listJsonConfigs = [];
        private readonly EventHandlerNWCExportUiArg _eventHandlerNWCExportUiArg;
        private readonly EventHandlerNWCExportBatchUiArg _eventHandlerNWCExportBatchUiArg;
        public NWCExportUi(EventHandlerNWCExportUiArg eventHandlerNWCExportUiArg, EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg)
        {
            InitializeComponent();
            DataContext = listBoxItems;

            _eventHandlerNWCExportUiArg = eventHandlerNWCExportUiArg;
            _eventHandlerNWCExportBatchUiArg = eventHandlerNWCExportBatchUiArg;
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
                    NWCForm form = JsonSerializer.Deserialize<NWCForm>(file);
                    NWCFormDeserilaizer(form);
                }
                catch
                {
                    MessageBox.Show("Неверная схема файла");
                }
            }
        }
        public void NWCFormDeserilaizer(NWCForm form)
        {
            CheckBoxConvertElementProperties.IsChecked = form.ConvertElementProperties;
            CheckBoxDivideFileIntoLevels.IsChecked = form.DivideFileIntoLevels;
            CheckBoxExportElementIds.IsChecked = form.ExportElementIds;
            CheckBoxExportLinks.IsChecked = form.ExportLinks;
            CheckBoxExportParts.IsChecked = form.ExportParts;
            CheckBoxExportRoomAsAttribute.IsChecked = form.ExportRoomAsAttribute;
            CheckBoxExportRoomGeometry.IsChecked = form.ExportRoomGeometry;
            CheckBoxExportUrls.IsChecked = form.ExportUrls;
            CheckBoxFindMissingMaterials.IsChecked = form.FindMissingMaterials;
            TextBoxExportScopeViewName.Text = form.ViewName;
            TextBoxFolder.Text = form.DestinationFolder;
            TextBoxPrefix.Text = form.Prefix;
            TextBoxPostfix.Text = form.Postfix;

            NavisworksExportScope navisworksExportScope = form.ExportScope;

            //switch (navisworksExportScope)
            //{
            //    case NavisworksExportScope.Model:
            //        RadioBattonExportScopeModel.IsChecked = true;
            //        break;
            //    case NavisworksExportScope.View:
            //        RadioButtonExportScopeView.IsChecked = true;
            //        break;
            //}

            listBoxItems.Clear();
            foreach (string file in form.RVTFiles)
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                ListBoxItem listBoxItem = new() { Content = file, Background = Brushes.White };
                if (!listBoxItems.Any(cont => cont.Content.ToString() == file)
                    || file.EndsWith(".rvt", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    listBoxItems.Add(listBoxItem);
                }
            }

            CheckBoxConvertLights.IsChecked = form.ConvertLights;
            CheckBoxConvertLinkedCADFormats.IsChecked = form.ConvertLinkedCADFormats;
            TextBoxFacetingFactor.Text = form.FacetingFactor.ToString();

            ComboBoxCoordinates.SelectedIndex = (int)form.Coordinates;
            ComboBoxParameters.SelectedIndex = (int)form.Parameters;
        }

        private void ButtonSaveList_Click(object sender, RoutedEventArgs e)
        {
            NWCForm form = NWCFormSerializer();

            SaveFileDialog saveFileDialog = new()
            {
                FileName = "ConfigBatchExportNWC",
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

        private NWCForm NWCFormSerializer()
        {
            string coordinates = ((ComboBoxItem)ComboBoxCoordinates.SelectedItem).Content.ToString();
            //bool exportScope = (bool)RadioBattonExportScopeModel.IsChecked;
            string parameters = ((ComboBoxItem)ComboBoxParameters.SelectedItem).Content.ToString();

            if (double.TryParse(TextBoxFacetingFactor.Text, out double facetingFactor))
            {
                facetingFactor = double.Parse(TextBoxFacetingFactor.Text);
            }
            else
            {
                facetingFactor = 1.0;
            }

            NWCForm form = new()
            {
                ConvertElementProperties = (bool)CheckBoxConvertElementProperties.IsChecked,
                DivideFileIntoLevels = (bool)CheckBoxDivideFileIntoLevels.IsChecked,
                ExportElementIds = (bool)CheckBoxExportElementIds.IsChecked,
                ExportLinks = (bool)CheckBoxExportLinks.IsChecked,
                ExportParts = (bool)CheckBoxExportParts.IsChecked,
                ExportRoomAsAttribute = (bool)CheckBoxExportRoomAsAttribute.IsChecked,
                ExportRoomGeometry = (bool)CheckBoxExportRoomGeometry.IsChecked,
                ExportUrls = (bool)CheckBoxExportUrls.IsChecked,
                FindMissingMaterials = (bool)CheckBoxFindMissingMaterials.IsChecked,
                ViewName = TextBoxExportScopeViewName.Text,
                DestinationFolder = TextBoxFolder.Text,
                Prefix = TextBoxPrefix.Text,
                Postfix = TextBoxPostfix.Text,
                RVTFiles = listBoxItems.Select(cont => cont.Content.ToString()).ToList(),
                ConvertLights = (bool)CheckBoxConvertLights.IsChecked,
                ConvertLinkedCADFormats = (bool)CheckBoxConvertLinkedCADFormats.IsChecked,
                FacetingFactor = facetingFactor
            };

            switch (coordinates)
            {
                case "Общие":
                    form.Coordinates = NavisworksCoordinates.Shared;
                    break;
                case "Внутренние для проекта":
                    form.Coordinates = NavisworksCoordinates.Internal;
                    break;
            }

            //switch (exportScope)
            //{
            //    case true:
            //        form.ExportScope = NavisworksExportScope.Model;
            //        break;
            //    case false:
            //        form.ExportScope = NavisworksExportScope.View;
            //        break;
            //}

            switch (parameters)
            {
                case "Все":
                    form.Parameters = NavisworksParameters.All;
                    break;
                case "Объекты":
                    form.Parameters = NavisworksParameters.Elements;
                    break;
                case "Нет":
                    form.Parameters = NavisworksParameters.None;
                    break;
            }

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
            FolderBrowserDialog folderBrowserDialog = new() { SelectedPath = TextBoxFolder.Text };
            DialogResult result = folderBrowserDialog.ShowDialog();
            string folderPath = folderBrowserDialog.SelectedPath;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxFolder.Text = folderPath;
            }
        }

        private void TextBoxFolder_TextChanged(object sender, TextChangedEventArgs e) { }
        private void ComboBoxCoordinates_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void CheckBoxDivideFileIntoLevels_Checked(object sender, RoutedEventArgs e) { }
        private void RadioBattonExportScopeModel_Checked(object sender, RoutedEventArgs e) { }
        private void RadioButtonExportScopeView_Checked(object sender, RoutedEventArgs e) { }
        private void ButtonOK_Click(object sender, RoutedEventArgs e) => _eventHandlerNWCExportUiArg.Raise(this);
        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
        private void ButtonErase_Click(object sender, RoutedEventArgs e)
        {
            listBoxItems.Clear();
            TextBoxFolder.Clear();
        }
        private void ButtonLoadJson_Click(object sender, RoutedEventArgs e)
        {
            listJsonConfigs.Clear();
            ListBoxJsonConfigs.Items.Clear();

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
                    listJsonConfigs = JsonSerializer.Deserialize<List<string>>(file);
                }
                catch
                {
                    MessageBox.Show("Неверная схема файла");
                }
            }

            foreach (string config in listJsonConfigs)
            {
                if (config.EndsWith(".json"))
                    ListBoxJsonConfigs.Items.Add(config);
            }
        }
        private void ButtonStartJson_Click(object sender, RoutedEventArgs e) => _eventHandlerNWCExportBatchUiArg.Raise(this);
        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            const string msg = "\tПлагин предназначен для пакетного экспорта файлов в формат NWC." +
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
                 "\tЗапустите экспорт кнопкой \"ОК\"." +
                 "\n\n" +
                 "**********************************************" +
                 "\n\n" +
                 "\tЕсли у вас есть несколько сохранённых конфигураций, то можно использовать пакетный экспорт второго уровня." +
                 "\n" +
                 "\tКнопкой \"Загрузить конфиги\" загрузите список (.JSON) с путями к конфигурациям в формате (.JSON). " +
                 "Структура списка выглядит следующим образом: \n[\n\t\"path\\\\config.json\",\n\t\"path\\\\config2.json\",\n\t\"path\\\\config3.json\"\n]" +
                 "\n" +
                 "\tКнопкой \"Начать\" запустите пакетный экспорт второго уровня, который экспортирует несколько объектов с соответствующими им настройками.";
            MessageBox.Show(msg, "Справка");
        }
    }
}