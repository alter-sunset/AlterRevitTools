namespace AlterTools.BatchExport.Utils.Extensions;

public static class ElementExtensions
{
    public static bool IsPhysicalElement(this Element el)
    {
        return el.Category is not null
               && !el.ViewSpecific
               && el.Category.CategoryType is CategoryType.Model
               && el.Category.CanAddSubcategory;
    }
}