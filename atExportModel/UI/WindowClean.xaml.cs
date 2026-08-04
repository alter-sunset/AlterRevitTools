using System.Windows;

namespace AlterTools.atExportModel.UI;

public partial class WindowClean : Window
{
    public WindowClean(ViewModelClean viewModelClean)
    {
        InitializeComponent();
        DataContext = viewModelClean;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e) => Close();
}