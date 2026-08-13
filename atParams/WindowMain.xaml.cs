namespace AlterTools.atParams;

public partial class WindowMain
{
    public WindowMain(ViewModelMain viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}