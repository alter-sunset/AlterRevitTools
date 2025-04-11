using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;
using System.Linq;

namespace AlterTools.BatchExport.Views.Params
{
    public class ParamsViewModel : ViewModelBase, IConfigParams
    {
        public ParamsViewModel(EventHandlerParams eventHandlerParams)
        {
            EventHandlerBase = eventHandlerParams;
        }

        private string _paramsNames = "";
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
    }
}