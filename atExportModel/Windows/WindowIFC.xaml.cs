using System.Windows;

namespace AlterTools.atExportModel.Windows;

public partial class WindowIFC
{
    public WindowIFC(ViewModelIFC viewModelIFC)
    {
        InitializeComponent();
        DataContext = viewModelIFC;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e) => Close();
}