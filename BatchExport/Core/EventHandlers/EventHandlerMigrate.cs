using System.Collections.Generic;
using System.Windows;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Migrate;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.EventHandlers;

public class EventHandlerMigrate : EventHandlerBase
{
    protected override void Execute(UIApplication uiApp, IConfigBase iConfigBase)
    {
        if (iConfigBase is not MigrateViewModel migrateVm) return;

        if (!MigrateHelper.IsConfigPathValid(migrateVm.ConfigPath))
        {
            MessageBox.Show(Resources.Resources.Migrate_NoConfig);
            return;
        }

        List<string> failedFiles = MigrateHelper.ProcessFiles(migrateVm.ConfigPath, uiApp.Application);

        string msg = failedFiles.Count > 0
            ? $"{Resources.Resources.Const_TaskCompleted}" +
              $"\n{Resources.Resources.Migrate_DidntCopy}" +
              $"\n{string.Join("\n", failedFiles)}"
            : Resources.Resources.Const_TaskCompleted;

        migrateVm.Finisher("MigrateModelsFinished", msg);
    }
}