using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.IFC
{
    public partial class IFCExportView : WindowBase
    {
        public IFCExportView(EventHandlerIFC eventHandlerIFC)
        {
            InitializeComponent();
            DataContext = new IFC_ViewModel(eventHandlerIFC);
        }
    }
}