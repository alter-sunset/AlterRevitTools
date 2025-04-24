using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Core.Commands;

public class CommandAvailability : IExternalCommandAvailability
{
    public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
    {
        return true;
    }
}