using VLS.BatchExport.Source.EventHandlers;
using VLS.BatchExport.Views.Base;

namespace VLS.BatchExport.Views.Migrate
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class MigrateModelsView : WindowBase
    {
        public MigrateModelsView(EventHandlerMigrate eventHandlerMigrate)
        {
            InitializeComponent();
            DataContext = new MigrateViewModel(eventHandlerMigrate);
        }
    }
}