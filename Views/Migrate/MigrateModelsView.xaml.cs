using VLS.BatchExportNet.Source.EventHandlers;

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