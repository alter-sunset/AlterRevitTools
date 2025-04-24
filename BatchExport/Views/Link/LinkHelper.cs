using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlterTools.BatchExport.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using InvalidOperationException = Autodesk.Revit.Exceptions.InvalidOperationException;

namespace AlterTools.BatchExport.Views.Link;

internal static class LinkHelper
{
    private const string DiffCoord =
        "Обнаружено различие систем координат. Выполнить получение коордианат из файла?";

    internal static void CreateLinks(this LinkViewModel linkViewModel, UIApplication uiApp)
    {
        Document doc = uiApp.ActiveUIDocument.Document;

        bool isCurrentWorkset = linkViewModel.IsCurrentWorkset;
        bool setWorksetId = !isCurrentWorkset && 0 < linkViewModel.Worksets.Length;

        List<Entry> entries = linkViewModel.Entries
            .Where(entry => !string.IsNullOrWhiteSpace(entry.Name) && File.Exists(entry.Name))
            .OrderBy(entry => entry.SelectedWorkset?.Name ?? string.Empty)
            .ToList();

        LinkProps props = new(doc.GetWorksetTable(), setWorksetId, linkViewModel.PinLinks,
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

        tr.Start($"Link {entry.Name}");

        if (props.SetWorksetId) props.WorksetTable.SetActiveWorksetId(entry.SelectedWorkset.Id);

        RevitLinkInstance revitLinkInstance;
        LinkLoadResult linkLoadResult = null;

        try
        {
            linkLoadResult = RevitLinkType.Create(doc, linkPath, revitLinkOptions);
            revitLinkInstance =
                RevitLinkInstance.Create(doc, linkLoadResult.ElementId, entry.SelectedImportPlacement);
            revitLinkInstance.Pinned = props.PinLink;

            tr.Commit();
        }
        catch (InvalidOperationException)
        {
            if (null == linkLoadResult)
            {
                tr.RollBack();
                return;
            }

            revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadResult.ElementId, ImportPlacement.Origin);
            YesNoTaskDialog(DiffCoord, () => doc.AcquireCoordinates(revitLinkInstance.Id));
            revitLinkInstance.Pinned = props.PinLink;

            tr.Commit();
        }
        catch
        {
            tr.RollBack();
        }
    }

    private static void YesNoTaskDialog(string msg, Action action)
    {
        TaskDialogResult result =
            TaskDialog.Show("Ошибка", msg, TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);
        if (TaskDialogResult.Yes == result) action?.Invoke();
    }
}