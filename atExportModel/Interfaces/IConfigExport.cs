using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigExport
{
    public bool ExportRVT { get; set; }

    // public bool AsTransmit { get; set; }
    public RvtExportMode RvtExportMode { get; set; }

    public bool ExportNWC { get; set; }
    public ConfigNWC ConfigNWC { get; set; }

    public bool ExportIFC { get; set; }
    public ConfigIFC ConfigIFC { get; set; }
    public ConfigIFCAdd ConfigIFCAdditionalFields { get; set; }

    public bool CleanModel { get; set; }
    public ConfigClean ConfigClean { get; set; }

    public string ViewName { get; set; }
    public string FolderPathRVT { get; set; }
    public string FolderPathNWC { get; set; }
    public string FolderPathIFC { get; set; }
}