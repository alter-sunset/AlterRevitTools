using System.Windows;

namespace AlterTools.atExportModel;

public partial class MainWindow
{
    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}