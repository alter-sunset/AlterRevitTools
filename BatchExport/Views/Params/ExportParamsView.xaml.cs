using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.Params
{
    public partial class ExportParamsView
    {
        public ExportParamsView(EventHandlerParams eventHandlerParams)
        {
            InitializeComponent();
            DataContext = new ParamsViewModel(eventHandlerParams);
        }
    }
}