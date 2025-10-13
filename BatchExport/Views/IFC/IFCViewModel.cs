using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Utils;
using AlterTools.BatchExport.Views.Base;
using Autodesk.Revit.DB;

namespace AlterTools.BatchExport.Views.IFC;

public class IFCViewModel : ViewModelBaseExtended, IConfigIFC
{
    private bool _exportBaseQuantities;

    private RelayCommand _loadMappingCommand;

    private string _mapping = string.Empty;

    private KeyValuePair<int, string> _selectedLevel =
        IFCContext.SpaceBoundaryLevels.FirstOrDefault(lev => lev.Key == 1);

    private KeyValuePair<IFCVersion, string> _selectedVersion =
        IFCContext.IFCVersions.FirstOrDefault(ver => ver.Key is IFCVersion.Default);

    private bool _wallAndColumnSplitting;

    public IFCViewModel(EventHandlerIFC eventHandlerIFC)
    {
        EventHandlerBase = eventHandlerIFC;
        HelpMessage = string.Join(Environment.NewLine,
            Strings.HelpIFCTitle,
            Strings.HelpLoad,
            Strings.HelpFolder,
            Strings.HelpNaming,
            Strings.HelpConfig,
            Strings.HelpStart);
    }

    public string Mapping
    {
        get => _mapping;
        set => SetProperty(ref _mapping, value);
    }

    public RelayCommand LoadMappingCommand => _loadMappingCommand ??= new RelayCommand(_ => LoadMapping());

    public static IReadOnlyDictionary<IFCVersion, string> IFCVersions => IFCContext.IFCVersions;

    public KeyValuePair<IFCVersion, string> SelectedVersion
    {
        get => _selectedVersion;
        set => SetProperty(ref _selectedVersion, value);
    }

    public IReadOnlyDictionary<int, string> SpaceBoundaryLevels { get; } = IFCContext.SpaceBoundaryLevels;

    public KeyValuePair<int, string> SelectedLevel
    {
        get => _selectedLevel;
        set => SetProperty(ref _selectedLevel, value);
    }

    public string FamilyMappingFile => _mapping;

    public bool ExportBaseQuantities
    {
        get => _exportBaseQuantities;
        set => SetProperty(ref _exportBaseQuantities, value);
    }

    public bool WallAndColumnSplitting
    {
        get => _wallAndColumnSplitting;
        set => SetProperty(ref _wallAndColumnSplitting, value);
    }

    public IFCVersion FileVersion => _selectedVersion.Key;

    public int SpaceBoundaryLevel => SelectedLevel.Key;

    private void LoadMapping()
    {
        using OpenFileDialog openFileDialog = DialogType.SingleText.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        try
        {
            Mapping = openFileDialog.FileName;
        }
        catch
        {
            MessageBox.Show(Strings.WrongScheme);
        }
    }

    protected override void LoadList()
    {
        OpenFileDialog openFileDialog = DialogType.SingleJson.OpenFileDialog();

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        using FileStream file = File.OpenRead(openFileDialog.FileName);

        IFCFormDeserializer(JsonHelper<IFCForm>.DeserializeConfig(file));
    }

    private void IFCFormDeserializer(IFCForm form)
    {
        if (form is null) return;

        FolderPath = form.FolderPath;
        NamePrefix = form.NamePrefix;
        NamePostfix = form.NamePostfix;
        WorksetPrefix = string.Join(";", form.WorksetPrefixes);
        Mapping = form.FamilyMappingFile;
        ExportBaseQuantities = form.ExportBaseQuantities;
        SelectedVersion = IFCVersions.FirstOrDefault(ver => ver.Key == form.FileVersion);
        WallAndColumnSplitting = form.WallAndColumnSplitting;
        ExportScopeView = form.ExportView;
        ViewName = form.ViewName;
        SelectedLevel = SpaceBoundaryLevels.FirstOrDefault(level => level.Key == form.SpaceBoundaryLevel);
        ListBoxItems = 
        [
            .. form.Files
                .FilterRevitFiles()
                .Select(DefaultListBoxItem)
        ];
        TurnOffLog = form.TurnOffLog;
    }

    protected override void SaveList()
    {
        using IFCForm form = IFCFormSerializer();

        SaveFileDialog saveFileDialog = DialogType.SingleJson.SaveFileDialog();

        if (saveFileDialog.ShowDialog() is not DialogResult.OK) return;

        string fileName = saveFileDialog.FileName;
        File.Delete(fileName);

        JsonHelper<IFCForm>.SerializeConfig(form, fileName);
    }

    private IFCForm IFCFormSerializer()
    {
        return new IFCForm
        {
            ExportBaseQuantities = ExportBaseQuantities,
            FamilyMappingFile = Mapping,
            FileVersion = SelectedVersion.Key,
            SpaceBoundaryLevel = SelectedLevel.Key,
            WallAndColumnSplitting = WallAndColumnSplitting,
            FolderPath = FolderPath,
            NamePrefix = NamePrefix,
            NamePostfix = NamePostfix,
            WorksetPrefixes = WorksetPrefixes,
            ExportView = ExportScopeView,
            ViewName = ViewName,
            Files = [.. ListBoxItems.Select(item => item.Content.ToString())],
            TurnOffLog = TurnOffLog
        };
    }
}