using Autodesk.Revit.DB;
using AlterTools.BatchExportNet.Views.Base;

namespace AlterTools.BatchExportNet.Views.IFC
{
    public interface IConfigIFC : IConfigBase_Extended
    {
        public string FamilyMappingFile { get; }
        public bool ExportBaseQuantities { get; set; }
        public bool WallAndColumnSplitting { get; set; }
        public IFCVersion FileVersion { get; }
        public int SpaceBoundaryLevel { get; }
    }
}