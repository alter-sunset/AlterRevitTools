using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.atUpdateRsLink;

public class CommandAvailability : IExternalCommandAvailability
{
    public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories) => true;
}