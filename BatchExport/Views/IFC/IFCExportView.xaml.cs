using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.IFC
{
    public partial class IfcExportView
    {
        public IfcExportView(EventHandlerIfc eventHandlerIfc)
        {
            InitializeComponent();
            DataContext = new IfcViewModel(eventHandlerIfc);
        }
    }
}