using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Migrate
{
    public partial class MigrateModelsView : WindowBase
    {
        public MigrateModelsView(EventHandlerMigrate eventHandlerMigrate)
        {
            InitializeComponent();
            DataContext = new MigrateViewModel(eventHandlerMigrate);
        }
    }
}