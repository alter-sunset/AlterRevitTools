using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.IFC
{
    public partial class IFCExportView
    {
        public IFCExportView(EventHandlerIFC eventHandlerIfc)
        {
            InitializeComponent();
            DataContext = new IFC_ViewModel(eventHandlerIfc);
        }
    }
}