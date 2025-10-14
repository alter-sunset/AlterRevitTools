using AlterTools.BatchExport.Utils;
using InvalidOperationException = Autodesk.Revit.Exceptions.InvalidOperationException;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace AlterTools.BatchExport.Views.Link;

internal static class LinkHelper
{
    private static string DiffCoord => Resources.Strings.DiffCoordError;

    internal static void CreateLinks(this LinkViewModel linkViewModel, UIApplication uiApp)
    {
        Document doc = uiApp.ActiveUIDocument.Document;

        bool isCurrentWorkset = linkViewModel.IsCurrentWorkset;
        bool setWorksetId = !isCurrentWorkset && linkViewModel.Worksets.Length > 0;

        List<Entry> entries = 
        [
            .. linkViewModel.Entries
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Name)
                                && File.Exists(entry.Name))
                .OrderBy(entry => entry.SelectedWorkset?.Name ?? string.Empty)
        ];

        LinkProps props = new(doc.GetWorksetTable(),
            setWorksetId,
            linkViewModel.PinLinks,
            linkViewModel.WorksetPrefixes);

        entries.ForEach(entry => TryCreateLink(doc, entry, props));
    }

    private static void TryCreateLink(Document doc, Entry entry, LinkProps props)
    {
        BasicFileInfo fileInfo = BasicFileInfo.Extract(entry.Name);

        ModelPath linkPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(entry.Name);

        RevitLinkOptions revitLinkOptions = fileInfo.IsWorkshared && 0 != props.WorksetPrefixes.Length
            ? new RevitLinkOptions(false, linkPath.CloseWorksets(props.WorksetPrefixes))
            : new RevitLinkOptions(false);

        using Transaction tr = new(doc);

        tr.Start($"{Resources.Strings.Link} {Path.GetFileNameWithoutExtension(entry.Name)}");

        if (props.SetWorksetId)
        {
            props.WorksetTable.SetActiveWorksetId(entry.SelectedWorkset.Id);
        }

        RevitLinkInstance revitLinkInstance;
        LinkLoadResult linkLoadResult = null;

        using DwgImportDialogSuppressor suppressor = new();
        
        try
        {
            linkLoadResult = RevitLinkType.Create(doc, linkPath, revitLinkOptions);
            revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, entry.SelectedImportPlacement);
            revitLinkInstance.Pinned = props.PinLink;

            tr.Commit();
        }
        catch (InvalidOperationException)
        {
            if (linkLoadResult is null)
            {
                tr.RollBack();
                return;
            }

            revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);
            ShowYesNoTaskDialog(DiffCoord, () => doc.AcquireCoordinates(revitLinkInstance.Id));
            revitLinkInstance.Pinned = props.PinLink;

            tr.Commit();
        }
        catch
        {
            tr.RollBack();
        }
    }

    private static void ShowYesNoTaskDialog(string msg, Action action)
    {
        TaskDialogResult result =
            TaskDialog.Show(Resources.Strings.Error,
                msg,
                TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
        if (result is TaskDialogResult.Yes)
        {
            action?.Invoke();
        }
    }
}