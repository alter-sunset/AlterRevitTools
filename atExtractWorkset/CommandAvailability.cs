using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.atExtractWorkset;

public class CommandAvailability : IExternalCommandAvailability
{
    public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories) => true;
}