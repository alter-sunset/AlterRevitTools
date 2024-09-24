using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.Base;

namespace VLS.BatchExportNet.Views.Migrate
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class MigrateModelsView : WindowBase
    {
        public MigrateModelsView(EventHandlerMigrateModelsVMArg eventHandlerMigrateModelsVMArg)
        {
            InitializeComponent();
            DataContext = new MigrateViewModel(eventHandlerMigrateModelsVMArg);
        }
    }
}