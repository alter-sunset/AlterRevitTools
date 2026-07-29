using System.Windows;
using AlterTools.atExportModel.Windows;

namespace AlterTools.atExportModel;

public partial class MainWindow
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }

    private void NWC_Settings_Click(object sender, RoutedEventArgs e)
    {
        WindowNWC window = new(ViewModel.ViewModelNWC);

        window.ShowDialog();
    }

    private void IFC_Settings_Click(object sender, RoutedEventArgs e)
    {
        WindowIFC window = new(ViewModel.ViewModelIFC);

        window.ShowDialog();
    }
}