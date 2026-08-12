using AlterTools.Resources;
using AlterTools.Utils;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atLink;

public class ExternalEventHandler : RevitEventWrapper<IConfigLink>
{
    public override void Execute(UIApplication uiApp, IConfigLink args)
    {
        if (args is null) return;

        using Application app = uiApp.Application;
        using ErrorSuppressor errorSuppressor = new(uiApp);

        args.CreateLinks(uiApp);

        MessageBox.Show(Strings.Done);
    }
}