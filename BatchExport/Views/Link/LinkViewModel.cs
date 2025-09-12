using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.Link;

public class LinkViewModel : ViewModelBase
{
    public static readonly ImportPlacement[] ImportPlacements =
    [
        ImportPlacement.Origin,
        ImportPlacement.Shared
    ];

    public readonly Workset[] Worksets;

    private ObservableCollection<Entry> _entries = [];

    private bool _isCurrentWorkset;

    private bool _pinLinks = true;

    private Entry _selectedEntry;

    private string _worksetPrefix = string.Empty;

    public LinkViewModel(EventHandlerLink eventHandlerLink, Workset[] worksets)
    {
        Worksets = worksets;
        EventHandlerBase = eventHandlerLink;
        HelpMessage = Help.GetHelpDictionary()
            .GetResultMessage(HelpMessageType.LinkTitle,
                HelpMessageType.Load,
                HelpMessageType.List,
                HelpMessageType.Start);
    }

    public bool IsCurrentWorkset
    {
        get => _isCurrentWorkset;
        set => SetProperty(ref _isCurrentWorkset, value);
    }

    public bool PinLinks
    {
        get => _pinLinks;
        set => SetProperty(ref _pinLinks, value);
    }

    public override string[] Files => [.. Entries.Select(e => e.Name)];

    public ObservableCollection<Entry> Entries
    {
        get => _entries;
        set => SetProperty(ref _entries, value);
    }

    public Entry SelectedEntry
    {
        get => _selectedEntry;
        set => SetProperty(ref _selectedEntry, value);
    }

    public string WorksetPrefix
    {
        get => _worksetPrefix;
        set => SetProperty(ref _worksetPrefix, value);
    }

    public string[] WorksetPrefixes => _worksetPrefix.SplitBySemicolon();

    public void UpdateSelectedEntries(Entry sourceEntry, bool isWorkset)
    {
        foreach (Entry entry in Entries.Where(en => en != sourceEntry && en.IsSelected))
        {
            if (isWorkset)
            {
                entry.SelectedWorkset = sourceEntry.SelectedWorkset;
            }
            else
            {
                entry.SelectedImportPlacement = sourceEntry.SelectedImportPlacement;
            }
        }
    }

    protected override void LoadList()
    {
        using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        IEnumerable<string> files = File.ReadLines(openFileDialog.FileName).FilterRevitFiles();

        Entries = [.. files.Select(file => new Entry(this, file))];

        if (!Entries.Any())
        {
            MessageBox.Show(NoFiles);
        }

        FolderPath = Path.GetDirectoryName(openFileDialog.FileName);
    }

    protected override void Load()
    {
        using OpenFileDialog openFileDialog = DialogType.MultiRevit.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        HashSet<string> existingFiles = [.. Files];

        openFileDialog.FileNames.Where(file => !existingFiles.Contains(file))
            .Distinct()
            .Select(file => new Entry(this, file))
            .ToList()
            .ForEach(Entries.Add);
    }

    protected override void SaveList()
    {
        SaveFileDialog saveFileDialog = DialogType.RevitList.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);
        File.WriteAllLines(fileName, Files);

        FolderPath = Path.GetDirectoryName(saveFileDialog.FileName);
    }

    protected override void DeleteSelectedItems()
    {
        Entries.Where(entry => entry.IsSelected)
            .ToList()
            .ForEach(entry => Entries.Remove(entry));
    }

    protected override void Erase() => Entries.Clear();
}