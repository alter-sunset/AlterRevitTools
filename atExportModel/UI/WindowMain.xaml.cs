namespace AlterTools.atExportModel.UI;

public partial class WindowMain
{
    public WindowMain(ViewModelMain viewModelMain)
    {
        InitializeComponent();
        DataContext = viewModelMain;
    }
}