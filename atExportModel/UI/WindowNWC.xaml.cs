using System.Windows;

namespace AlterTools.atExportModel.UI;

public partial class WindowNWC
{
    public WindowNWC(ViewModelNWC viewModelNWC)
    {
        InitializeComponent();
        DataContext = viewModelNWC;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e) => Close();
}