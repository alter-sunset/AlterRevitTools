using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Migrate
{
    /// <summary>
    /// Interaction logic for DetachModelsUi.xaml
    /// </summary>
    public partial class MigrateModelsUi : Window
    {
        public MigrateModelsUi(EventHandlerMigrateModelsVMArg eventHandlerMigrateModelsUiArg)
        {
            InitializeComponent();
            DataContext = new MigrateViewModel(eventHandlerMigrateModelsUiArg);
        }
    }
}