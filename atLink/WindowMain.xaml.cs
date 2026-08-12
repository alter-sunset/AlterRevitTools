namespace AlterTools.atLink;

public partial class WindowMain
{
    public WindowMain(ViewModelMain viewModelMain)
    {
        DataContext = viewModelMain;
        InitializeComponent();
    }
}