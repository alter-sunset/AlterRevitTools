using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Migrate
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