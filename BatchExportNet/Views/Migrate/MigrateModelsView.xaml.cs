using AlterTools.BatchExportNet.Source.EventHandlers;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.Migrate
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