using System.Collections.ObjectModel;
using Autodesk.Revit.DB;

namespace AlterTools.atLink;

public interface IConfigLink
{
    public ObservableCollection<Entry> Entries { get; set; }
    public Workset[] Worksets { get; }
    public bool IsCurrentWorkset { get; set; }
    public bool PinLinks { get; set; }
    public string[] WorksetPrefixes { get; }
    public string FolderPath { get; set; }
}