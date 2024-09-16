using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using VLS.BatchExportNet.Utils;

namespace VLS.BatchExportNet.Source
{
    public class CommandAvailability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories) => true;
    }
    static class ExternalCommandWrapper
    {
        internal static Result Execute(ref string message, Forms form)
        {
            try
            {
                ViewHelper.ShowForm(form);
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
            ExternalCommandWrapper.Execute(ref message, Forms.NWC);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsToIFC : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref message, Forms.IFC);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsDetached : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref message, Forms.Detach);
    }

    [Transaction(TransactionMode.Manual)]
    public class ExportModelsTransmitted : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref message, Forms.Transmit);
    }

    [Transaction(TransactionMode.Manual)]
    public class MigrateModels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref message, Forms.Migrate);
    }
    [Transaction(TransactionMode.Manual)]
    public class LinkModels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) =>
            ExternalCommandWrapper.Execute(ref message, Forms.Link);
    }
}
