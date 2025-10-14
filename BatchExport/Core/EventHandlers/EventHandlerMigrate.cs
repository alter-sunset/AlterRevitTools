using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Migrate;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerMigrate : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not MigrateViewModel migrateVm) return;

        if (!MigrateHelper.IsConfigPathValid(migrateVm.ConfigPath))
        {
            MessageBox.Show(Strings.MigrateNoConfig);
            return;
        }

        List<string> failedFiles = MigrateHelper.ProcessFiles(migrateVm.ConfigPath, uiApp.Application);

        string msg = failedFiles.Count > 0
            ? $"{Strings.TaskCompleted}" +
              $"\n{Strings.MigrateDidntCopy}" +
              $"\n{string.Join("\n", failedFiles)}"
            : Strings.TaskCompleted;

        migrateVm.Finisher("MigrateModelsFinished", msg);
    }
}