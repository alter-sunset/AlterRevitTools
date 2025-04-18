using AlterTools.BatchExport.Core.EventHandlers;

namespace AlterTools.BatchExport.Views.Migrate
{
    public partial class MigrateModelsView
    {
        public MigrateModelsView(EventHandlerMigrate eventHandlerMigrate)
        {
            InitializeComponent();
            DataContext = new MigrateViewModel(eventHandlerMigrate);
        }
    }
}