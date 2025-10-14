using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Base;

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
        OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();
        if (openFileDialog.ShowDialog() is DialogResult.OK)
        {
            ConfigPath = openFileDialog.FileName;
        }
    }
}