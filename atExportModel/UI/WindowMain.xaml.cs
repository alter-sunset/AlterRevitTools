using System.Windows;

namespace AlterTools.atExportModel.UI;

public partial class WindowMain
{
    private ViewModelMain ViewModel => (ViewModelMain)DataContext;

    public WindowMain(ViewModelMain viewModelMain)
    {
        InitializeComponent();
        DataContext = viewModelMain;
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

    private void Clean_Setting_Click(object sender, RoutedEventArgs e)
    {
        WindowClean window = new(ViewModel.ViewModelClean);

        window.ShowDialog();
    }
}