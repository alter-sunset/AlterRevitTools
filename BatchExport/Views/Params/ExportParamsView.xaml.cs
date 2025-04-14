using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Params
{
    public partial class ExportParamsView : WindowBase
    {
        public ExportParamsView(EventHandlerParams eventHandlerParams)
        {
            InitializeComponent();
            DataContext = new ParamsViewModel(eventHandlerParams);
        }
    }
}