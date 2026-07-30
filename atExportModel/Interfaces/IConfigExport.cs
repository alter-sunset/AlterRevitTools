using AlterTools.atExportModel.Enums;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigExport
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

    public string[] InputFiles { get; set; }

    //maybe later
    // public bool Report { get; set; }
    // public IConfigReport ConfigReport { get; set; }
}