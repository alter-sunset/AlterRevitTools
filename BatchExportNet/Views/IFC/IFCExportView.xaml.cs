using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.IFC
{
    /// <summary>
    /// Interaction logic for NWCExportView.xaml
    /// </summary>
    public partial class IFCExportView : WindowBase
    {
        public IFCExportView(EventHandlerIFC eventHandlerIFC)
        {
            InitializeComponent();
            DataContext = new IFC_ViewModel(eventHandlerIFC);
        }
    }
}