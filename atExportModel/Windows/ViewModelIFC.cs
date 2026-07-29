using AlterTools.atExportModel.Configs;
using AlterTools.Utils.MVVM;

namespace AlterTools.atExportModel.Windows;

public class ViewModelIFC : NotifyPropertyChanged
{
    public ConfigIFC Config { get; set; }

    public ViewModelIFC(ConfigIFC config)
    {
        Config = config;
    }
}