using Autodesk.Revit.DB;

namespace AlterTools.atLink;

public class LinkProps(WorksetTable table, bool setWorksetId, bool pinLinks, string[] worksetPrefixes)
{
    public WorksetTable WorksetTable { get; } = table;
    public bool SetWorksetId { get; } = setWorksetId;
    public bool PinLink { get; } = pinLinks;
    public string[] WorksetPrefixes { get; } = worksetPrefixes;
}