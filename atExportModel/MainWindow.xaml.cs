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

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        WindowNWC window = new(ViewModel.ViewModelNWC);

        window.ShowDialog();
    }
}