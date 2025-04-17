using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.NWC
{
    public class NwcForm : NavisworksExportOptions
    {
        public string FolderPath { get; init; }

        public string NamePrefix { get; init; }

        public string NamePostfix { get; init; }

        public string[] WorksetPrefixes { get; init; }

        public string[] Files { get; init; }

        public string ViewName { get; init; }

        public new bool ConvertLights { get; init; }

        public new bool ConvertLinkedCADFormats { get; init; }

        public new double FacetingFactor { get; init; }

        public new bool ViewId { get; set; }
    }
}