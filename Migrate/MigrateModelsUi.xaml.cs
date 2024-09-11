using Autodesk.Revit.UI;
using System.Windows;
using System.Windows.Forms;
using VLS.BatchExportNet.Utils;
using MessageBox = System.Windows.Forms.MessageBox;

namespace VLS.BatchExportNet.Migrate
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class MigrateModelsUi : Window
    {
        private readonly EventHandlerMigrateModelsUiArg _eventHandlerMigrateModelsUiArg;

        public MigrateModelsUi(UIApplication uiApp, EventHandlerMigrateModelsUiArg eventHandlerMigrateModelsUiArg)
        {
            InitializeComponent();
            _eventHandlerMigrateModelsUiArg = eventHandlerMigrateModelsUiArg;
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                DefaultExt = ".json",
                Filter = "Файл JSON (.json)|*.json"
            };

            DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxConfig.Text = openFileDialog.FileName;
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            _eventHandlerMigrateModelsUiArg.Raise(this);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            _ = MessageBox.Show(text: "\tПлагин предназначен для миграции проекта в новое место с сохранением структуры связей, как внутри папок, так и внутри самих моделей." +
                "\n" +
                "\tОткройте или вставьте ссылку на Json конфиг, который хранит в себе структуру типа Dictionary<string, string>," +
                "\n" +
                "где первый string - текущий путь к файлу, второй - новый путь." +
                "\n" +
                "Пример:" +
                "\n" +
                "{ \"C:\\oldfile.rvt\": \"C:\\newfile.rvt\",}");
        }
    }
}
