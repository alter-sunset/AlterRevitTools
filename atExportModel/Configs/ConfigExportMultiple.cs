using System.Collections.ObjectModel;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;

namespace AlterTools.atExportModel.Configs;

public class ConfigExportMultiple : IConfigExportMultiple
{
    public bool ExportRVT { get; set; }
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
    public ObservableCollection<string> InputFiles { get; set; }

    public ConfigExportMultiple()
    {
    }

    public ConfigExportMultiple(IConfigExportMultiple parent)
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
        InputFiles = parent.InputFiles;
    }
}