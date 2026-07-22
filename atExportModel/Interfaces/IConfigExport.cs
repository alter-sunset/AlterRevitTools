using AlterTools.atExportModel.Enums;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigExport
{
    public bool ExportRvt { get; set; }
    public RvtExportMode RvtExportMode { get; set; }

    public bool ExportNwc { get; set; }
    public IConfigNWC ConfigNWC { get; set; }

    public bool ExportIfc { get; set; }
    public IConfigIFC ConfigIFC { get; set; }

    public bool CleanModel { get; set; }
    public IConfigClean ConfigClean { get; set; }

    //maybe later
    // public bool Report { get; set; }
    // public IConfigReport ConfigReport { get; set; }
}