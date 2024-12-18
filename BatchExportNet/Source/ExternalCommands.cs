using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System;
using AlterTools.BatchExportNet.Utils;

namespace AlterTools.BatchExportNet.Source
{
    public class CommandAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories) => true;
    }
    static class ExternalCommandWrapper
    {
        internal static Result Execute(ref string msg, Forms form, UIApplication uiApp = null)
        {
            try
            {
                form.ShowForm(uiApp);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return Result.Failed;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsToNWC : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref msg, Forms.NWC);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsToIFC : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref msg, Forms.IFC);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsDetached : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref msg, Forms.Detach);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsTransmitted : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref msg, Forms.Transmit);
    }

    [Transaction(TransactionMode.Manual)]
    public class MigrateModels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref msg, Forms.Migrate);
    }
    [Transaction(TransactionMode.Manual)]
    public class LinkModels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref msg, Forms.Link, commandData.Application);
    }
}