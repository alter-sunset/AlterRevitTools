using System.Windows;

namespace AlterTools.atExportModel.UI;

public partial class WindowClean : Window
{
    public WindowClean()
    {
        InitializeComponent();
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e) => Close();
}