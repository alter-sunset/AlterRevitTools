using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;
using System.Linq;
using System.Windows.Forms;

namespace AlterTools.BatchExport.Views.Params
{
    public class ParamsViewModel : ViewModelBase, IConfigParams
    {
        public ParamsViewModel(EventHandlerParams eventHandlerParams)
        {
            EventHandlerBase = eventHandlerParams;
        }

        private string _paramsNames = "ADSK_Этаж;ADSK_Номер здания;ADSK_Комплект чертежей;";
        public string ParamsNames
        {
            get => _paramsNames;
            set => SetProperty(ref _paramsNames, value);
        }
        public string[] ParametersNames => ParamsNames.Split(';')
            .Select(e => e.Trim())
            .Distinct()
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToArray();


        private string _csvPath = "";
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
    }
}