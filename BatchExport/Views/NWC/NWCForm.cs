using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.NWC;

public class NWCForm : NavisworksExportOptions
{
    public string FolderPath { get; set; }
    public string NamePrefix { get; set; }
    public string NamePostfix { get; set; }
    public string[] WorksetPrefixes { get; set; }
    public string[] Files { get; set; }
    public string ViewName { get; set; }
    public new bool ConvertLights { get; set; }
    public new bool ConvertLinkedCADFormats { get; set; }
    public new double FacetingFactor { get; set; }
    public new bool ViewId { get; set; }
    public bool TurnOffLog {get; set;}
}