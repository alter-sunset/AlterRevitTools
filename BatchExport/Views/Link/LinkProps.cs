using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.Link
{
    public class LinkProps(WorksetTable table, bool setWorksetId, bool pinLinks, string[] worksetPrefixes)
    {
        public WorksetTable WorksetTable { get; set; } = table;
        public bool SetWorksetId { get; set; } = setWorksetId;
        public bool PinLink { get; set; } = pinLinks;
        public string[] WorksetPrefixes { get; set; } = worksetPrefixes;
    }
}