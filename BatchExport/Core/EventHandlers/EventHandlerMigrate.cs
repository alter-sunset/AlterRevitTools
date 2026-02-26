using System.Text;
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

        StringBuilder sb = new(Strings.TaskCompleted);
        if (failedFiles.Count > 0)
        {
            sb.AppendLine(Strings.MigrateDidntCopy);
            sb.AppendLine(string.Join("\n", failedFiles));
        }

        migrateVm.FinishWork("MigrateModelsFinished", sb.ToString());
    }
}