using AlterTools.Resources;
using AlterTools.Utils;
using AlterTools.Utils.MVVM;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace AlterTools.atParams;

public class ExternalEventHandler : RevitEventWrapper<IConfigParams>
{
    public override void Execute(UIApplication uiApp, IConfigParams args)
    {
        using (CsvHelper csvHelper = new(args.CsvPath, ["ModelName", "ElementId", .. args.ParametersNames]))
        {
            using ErrorSuppressor errorSuppressor = new(uiApp);
            using Application app = uiApp.Application;

            foreach (string file in args.Files)
            {
                Helper.ExportParameters(file, app, args.ParametersNames, csvHelper);
            }
        }

        MessageBox.Show(Strings.Done);
    }
}