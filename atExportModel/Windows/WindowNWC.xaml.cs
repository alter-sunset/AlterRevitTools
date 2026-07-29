using System.Windows;
using AlterTools.atExportModel.Interfaces;
using Autodesk.Revit.DB;

namespace AlterTools.atExportModel.Windows;

public partial class WindowNWC : Window
{
    public WindowNWC(ViewModelNWC viewModelNWC)
    {
        InitializeComponent();
        DataContext = viewModelNWC;
    }

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}