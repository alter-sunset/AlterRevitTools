using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;

namespace AlterTools.atExportModel.Configs;

public class ConfigExportSingle : IConfigExportSingle
{
    public bool ExportRVT { get; set; }
    public RvtExportMode RvtExportMode { get; set; }
    public bool ExportNWC { get; set; }
    public IConfigNWC ConfigNWC { get; set; }
    public bool ExportIFC { get; set; }
    public IConfigIFC ConfigIFC { get; set; }
    public IConfigIFCAdditionalFields ConfigIFCAdditionalFields { get; set; }
    public bool CleanModel { get; set; }
    public IConfigClean ConfigClean { get; set; }
    public string ViewName { get; set; }
    public string FolderPathRVT { get; set; }
    public string FolderPathNWC { get; set; }
    public string FolderPathIFC { get; set; }
    public string FileName { get; set; }

    public ConfigExportSingle(IConfigExport parent)
    {
        ExportRVT = parent.ExportRVT;
        RvtExportMode = parent.RvtExportMode;
        ExportNWC = parent.ExportNWC;
        ConfigNWC = parent.ConfigNWC;
        ExportIFC = parent.ExportIFC;
        ConfigIFC = parent.ConfigIFC;
        ConfigIFCAdditionalFields = parent.ConfigIFCAdditionalFields;
        CleanModel = parent.CleanModel;
        ConfigClean = parent.ConfigClean;
        ViewName = parent.ViewName;
        FolderPathRVT = parent.FolderPathRVT;
        FolderPathNWC = parent.FolderPathNWC;
        FolderPathIFC = parent.FolderPathIFC;
    }
}