using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Utils;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atExportModel;

public class ExternalEventHandler : RevitEventWrapper<IConfigExportMultiple>
{
    public override void Execute(UIApplication uiApp, IConfigExportMultiple args)
    {
        if (args is null) return;

        using Application app = uiApp.Application;

        using ErrorSuppressor errorSuppressor = new(uiApp);

        ConfigExportSingle config = new(args);

        foreach (string file in args.InputFiles)
        {
            config.FileName = file;
            UtilsMain.ProcessModel(app, config);
        }
    }
}