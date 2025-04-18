using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AlterTools.BatchExport.Views.Params
{
    public class ParamsViewModel : ViewModelBase, IConfigParams
    {
        private const string DefaultParams = "ADSK_Этаж;ADSK_Номер здания;ADSK_Комплект чертежей;";

        public ParamsViewModel(EventHandlerParams eventHandlerParams)
        {
            EventHandlerBase = eventHandlerParams;
            HelpMessage = Help.GetHelpDictionary()
                              .GetResultMessage(HelpMessageType.ParamsTitle,
                                                HelpMessageType.Load,
                                                HelpMessageType.Config,
                                                HelpMessageType.Start);

            ParamsNames = DefaultParams;
        }

        private string _paramsNames;
        public string ParamsNames
        {
            get => _paramsNames;
            set => SetProperty(ref _paramsNames, value);
        }
        public string[] ParametersNames => _paramsNames.SplitBySemicolon();

        private string _csvPath = string.Empty;
        public string CsvPath
        {
            get => _csvPath;
            set => SetProperty(ref _csvPath, value);
        }

        private RelayCommand _browseCsvCommand;
        public RelayCommand BrowseCsvCommand => _browseCsvCommand ??= new RelayCommand(_ => BrowseCsv());
        private void BrowseCsv()
        {
            SaveFileDialog saveFileDialog = DialogType.SingleCsv.SaveFileDialog();

            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            CsvPath = saveFileDialog.FileName;
        }

        protected override void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

            if (DialogResult.OK != openFileDialog.ShowDialog()) return;

            using FileStream file = File.OpenRead(openFileDialog.FileName);

            ParamsFormDeserilaizer(JsonHelper<ParamsForm>.DeserializeConfig(file));
        }
        private void ParamsFormDeserilaizer(ParamsForm form)
        {
            if (null == form) return;

            ParamsNames = string.Join(";", form.ParametersNames);
            CsvPath = form.CsvPath;
            ListBoxItems = new ObservableCollection<ListBoxItem>(form.Files.FilterRevitFiles()
                                                                           .Select(DefaultListBoxItem));
        }

        protected override void SaveList()
        {
            ParamsForm form = ParamsFormSerializer();
            SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            string fileName = saveFileDialog.FileName;
            File.Delete(fileName);

            JsonHelper<ParamsForm>.SerializeConfig(form, fileName);
        }
        private ParamsForm ParamsFormSerializer() => new()
        {
            CsvPath = CsvPath,
            ParametersNames = ParametersNames,
            Files = ListBoxItems.Select(item => item.Content.ToString() ?? string.Empty)
                                .ToArray()
        };

        protected override void Erase()
        {
            ListBoxItems.Clear();
            ParamsNames = DefaultParams;
            CsvPath = string.Empty;
        }
    }
}