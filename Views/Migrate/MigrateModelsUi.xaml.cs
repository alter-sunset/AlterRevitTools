using System.Windows;
using VLS.BatchExportNet.Source;

namespace VLS.BatchExportNet.Views.Migrate
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class MigrateModelsUi : Window
    {
        public MigrateModelsUi(EventHandlerMigrateModelsUiArg eventHandlerMigrateModelsUiArg)
        {
            InitializeComponent();
            DataContext = new MigrateViewModel(eventHandlerMigrateModelsUiArg);
        }
    }
}