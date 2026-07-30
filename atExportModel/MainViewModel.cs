using AlterTools.atExportModel.Configs;
using AlterTools.atExportModel.Enums;
using AlterTools.atExportModel.Interfaces;
using AlterTools.atExportModel.Windows;
using AlterTools.Utils.MVVM;

namespace AlterTools.atExportModel;

public class MainViewModel : NotifyPropertyChanged, IConfigExport
{
    private readonly ExternalEventHandler _handler;

    public bool ExportRVT { get; set; } = false;
    public RvtExportMode RvtExportMode { get; set; }

    public bool ExportNWC { get; set; } = false;
    public IConfigNWC ConfigNWC { get; set; }
    public ViewModelNWC ViewModelNWC { get; }

    public bool ExportIFC { get; set; } = false;
    public IConfigIFC ConfigIFC { get; set; }
    public IConfigIFCAdditionalFields ConfigIFCAdditionalFields { get; set; }
    public ViewModelIFC ViewModelIFC { get; }

    public bool CleanModel { get; set; } = false;
    public IConfigClean ConfigClean { get; set; }

    public string ViewName { get; set; } = "Navisworks";
    public string FolderPathRVT { get; set; }
    public string FolderPathNWC { get; set; }
    public string FolderPathIFC { get; set; }

    public MainViewModel(ExternalEventHandler handler)
    {
        _handler = handler;

        ConfigNWC = new ConfigNWC();
        ViewModelNWC = new ViewModelNWC(ConfigNWC);

        ConfigIFC = new ConfigIFC();
        ConfigIFCAdditionalFields = (IConfigIFCAdditionalFields)ConfigIFC;
        ViewModelIFC = new ViewModelIFC((ConfigIFC)ConfigIFC);
    }
}