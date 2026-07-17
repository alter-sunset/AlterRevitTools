using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.Utils.MVVM;

namespace AlterTools.BatchExport.Views.Migrate;

public class MigrateViewModel : ViewModelBase
{
    private string _configPath;

    public MigrateViewModel(EventHandlerMigrate eventHandlerMigrate)
    {
        EventHandlerBase = eventHandlerMigrate;
        HelpMessage = Resources.Strings.HelpMigrate;
    }

    [UsedImplicitly]
    public string ConfigPath
    {
        get => _configPath;
        set => SetProperty(ref _configPath, value);
    }

    protected override void LoadList()
    {
        using OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        ConfigPath = openFileDialog.FileName;
    }
}