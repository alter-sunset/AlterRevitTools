using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace VLS.BatchExportNet.Source
{
    static class ExternalCommandWrapper
    {
        public static Result Execute(UIApplication app, ref string message, Forms form)
        {
            try
            {
                App.ShowForm(app, form);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsToNWC : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(commandData.Application, ref message, Forms.NWC);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsToIFC : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(commandData.Application, ref message, Forms.IFC);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsDetached : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(commandData.Application, ref message, Forms.Detach);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsTransmitted : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(commandData.Application, ref message, Forms.Transmit);
    }

    [Transaction(TransactionMode.Manual)]
    public class MigrateModels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(commandData.Application, ref message, Forms.Migrate);
    }
    [Transaction(TransactionMode.Manual)]
    public class LinkModels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(commandData.Application, ref message, Forms.Link);
    }
}
