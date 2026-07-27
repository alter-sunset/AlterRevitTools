using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.Utils.MVVM;

namespace AlterTools.atExportModel;

public class MainViewModel(ExternalEventHandler handler) : NotifyPropertyChanged, IConfigExport
{
    private readonly ExternalEventHandler _handler = handler;


    public bool ExportRVT { get; set; }
    public RvtExportMode RvtExportMode { get; set; }

    public bool ExportNWC { get; set; }
    public IConfigNWC ConfigNWC { get; set; }

    public bool ExportIFC { get; set; }
    public IConfigIFC ConfigIFC { get; set; }

    public bool CleanModel { get; set; }
    public IConfigClean ConfigClean { get; set; }
}