﻿using AlterTools.BatchExport.Core.EventHandlers;
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
        private const string DEFAULT_PARAMS = "ADSK_Этаж;ADSK_Номер здания;ADSK_Комплект чертежей;";

        public ParamsViewModel(EventHandlerParams eventHandlerParams)
        {
            EventHandlerBase = eventHandlerParams;
            HelpMessage = Help.GetHelpDictionary()
                              .GetResultMessage(HelpMessageType.ParamsTitle,
                                                HelpMessageType.Load,
                                                HelpMessageType.Config,
                                                HelpMessageType.Start);

            ParamsNames = DEFAULT_PARAMS;
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
        public virtual RelayCommand BrowseCsvCommand => _browseCsvCommand ??= new RelayCommand(obj => BrowseCsv());
        private void BrowseCsv()
        {
            SaveFileDialog saveFileDialog = DialogType.SingleCsv.SaveFileDialog();

            if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

            CsvPath = saveFileDialog.FileName;
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand => _loadListCommand ??= new RelayCommand(obj => LoadList());
        private void LoadList()
        {
            OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

            if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

            using FileStream file = File.OpenRead(openFileDialog.FileName);

            ParamsFormDeserilaizer(JsonHelper<ParamsForm>.DeserializeConfig(file));
        }
        private void ParamsFormDeserilaizer(ParamsForm form)
        {
            if (form is null) return;

            ParamsNames = string.Join(";", form.ParametersNames);
            CsvPath = form.CsvPath;
            ListBoxItems = new ObservableCollection<ListBoxItem>(form.Files.FilterRevitFiles()
                                                                           .Select(DefaultListBoxItem));
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand => _saveListCommand ??= new RelayCommand(obj => SaveList());
        private void SaveList()
        {
            ParamsForm form = ParamsFormSerializer();
            SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

            if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

            string fileName = saveFileDialog.FileName;
            File.Delete(fileName);

            JsonHelper<ParamsForm>.SerializeConfig(form, fileName);
        }
        private ParamsForm ParamsFormSerializer() => new()
        {
            CsvPath = CsvPath,
            ParametersNames = ParametersNames,
            Files = ListBoxItems.Select(cont => cont.Content.ToString() ?? string.Empty)
                                .ToArray()
        };

        private RelayCommand _eraseCommand;
        public override RelayCommand EraseCommand => _eraseCommand ??= new RelayCommand(obj =>
        {
            ListBoxItems.Clear();
            ParamsNames = DEFAULT_PARAMS;
            CsvPath = string.Empty;
        });
    }
}