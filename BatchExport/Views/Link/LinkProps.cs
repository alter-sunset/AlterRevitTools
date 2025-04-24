using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.Link;

public class LinkProps(WorksetTable table, bool setWorksetId, bool pinLinks, string[] worksetPrefixes)
{
    public WorksetTable WorksetTable { get; } = table;
    public bool SetWorksetId { get; } = setWorksetId;
    public bool PinLink { get; } = pinLinks;
    public string[] WorksetPrefixes { get; } = worksetPrefixes;
}