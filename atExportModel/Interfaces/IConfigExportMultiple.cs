using System.Collections.ObjectModel;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigExportMultiple : IConfigExport
{
    public ObservableCollection<string> InputFiles { get; set; }
}