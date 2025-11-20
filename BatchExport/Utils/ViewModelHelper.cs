using System.Windows;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Detach;
using AlterTools.BatchExport.Views.Link;
using AlterTools.BatchExport.Views.Params;
using AlterTools.BatchExport.Views.Transmit;
using MessageBox = System.Windows.MessageBox;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace AlterTools.BatchExport.Utils;

internal static class ViewModelHelper
{
    internal static bool IsEverythingFilled(this DetachViewModel detachVm)
    {
        return detachVm.IsListNotEmpty()
               && detachVm.IsRbModeOk()
               && detachVm.IsViewNameOk()
               && detachVm.IsMaskNameOk();
    }

    internal static bool IsEverythingFilled(this TransmitViewModel transmitVm)
    {
        return transmitVm.IsListNotEmpty()
               && transmitVm.IsFolderPathOk();
    }

    internal static bool IsEverythingFilled(this ParamsViewModel paramsVm)
    {
        return paramsVm.IsListNotEmpty()
               && paramsVm.IsCsvPathNotEmpty()
               && paramsVm.AreThereAnyParameters();
    }

    internal static bool IsEverythingFilled(this ViewModelBaseExtended vmBaseExt)
    {
        return vmBaseExt.IsListNotEmpty()
               && vmBaseExt.IsFolderPathOk()
               && vmBaseExt.IsViewNameOk();
    }

    internal static bool IsEverythingFilled(this LinkViewModel linkVm) => linkVm.IsListNotEmpty();

    private static bool IsListNotEmpty(this LinkViewModel linkVm) =>
        CheckCondition(linkVm.Entries.Count > 0, Strings.NoFiles);

    private static bool IsListNotEmpty(this ViewModelBase vmBase) =>
        CheckCondition(vmBase.ListBoxItems.Count > 0, Strings.NoFiles);

    private static bool IsFolderPathOk(this ViewModelBase vmBase)
    {
        string folderPath = vmBase.FolderPath;

        if (string.IsNullOrEmpty(folderPath))
        {
            return CheckCondition(false, Strings.NoFolder);
        }

        if (Uri.IsWellFormedUriString(folderPath, UriKind.RelativeOrAbsolute))
        {
            return CheckCondition(false, Strings.WrongFolder);
        }

        if (Directory.Exists(folderPath)) return true;

        MessageBoxResult result = MessageBox.Show(Strings.CreateFolderError,
            Strings.GoodEvening,
            MessageBoxButton.YesNo);

        if (result is MessageBoxResult.Yes)
        {
            Directory.CreateDirectory(folderPath);
            return true;
        }

        MessageBox.Show(Strings.ToHell);
        return false;
    }

    private static bool IsViewNameOk(this ViewModelBaseExtended vmBaseExt)
    {
        return CheckCondition(!vmBaseExt.ExportScopeView
                              || !string.IsNullOrEmpty(vmBaseExt.ViewName), Strings.NoViewName);
    }

    private static bool IsViewNameOk(this DetachViewModel detachVm)
    {
        return CheckCondition(!detachVm.CheckForEmptyView
                              || !string.IsNullOrEmpty(detachVm.ViewName), Strings.NoViewName);
    }

    private static bool IsRbModeOk(this DetachViewModel detachVm)
    {
        switch (detachVm.RadioButtonMode)
        {
            case 0:
                return CheckCondition(false, Strings.NoPathMode);

            case 1:
                return detachVm.IsFolderPathOk();

            case 2:
                if (string.IsNullOrEmpty(detachVm.MaskIn) || string.IsNullOrEmpty(detachVm.MaskOut))
                {
                    return CheckCondition(false, Strings.NoMaskPath);
                }

                if (!detachVm.ListBoxItems
                        .Select(item => item.Content)
                        .All(i => i.ToString()
                            !.Contains(detachVm.MaskIn)))
                {
                    return CheckCondition(false, Strings.WrongMask);
                }

                break;
        }

        return true;
    }

    private static bool IsMaskNameOk(this DetachViewModel detachVm)
    {
        return CheckCondition(!detachVm.IsToRename
                              || !string.IsNullOrEmpty(detachVm.MaskInName), Strings.NoMaskFile);
    }

    private static bool IsCsvPathNotEmpty(this ParamsViewModel paramsVm)
    {
        string csvPath = paramsVm.CsvPath;

        if (string.IsNullOrWhiteSpace(csvPath)
            || Uri.IsWellFormedUriString(csvPath, UriKind.Absolute)
            || !csvPath.EndsWith(".csv"))
        {
            return CheckCondition(false, Strings.NoCsv);
        }

        return true;
    }

    private static bool AreThereAnyParameters(this ParamsViewModel paramsVm)
    {
        return CheckCondition(paramsVm.ParametersNames.Length > 0, Strings.NoParameters);
    }

    private static bool CheckCondition(bool condition, string msg)
    {
        if (!condition)
        {
            MessageBox.Show(msg);
        }

        return condition;
    }

    /// <summary>
    ///     Default finisher method of most plugins,
    ///     that will show final TaskDialog and lock View until TaskDialog is closed
    /// </summary>
    /// <param name="vmBase">ViewModel to finalize</param>
    /// <param name="id">TaskDialog Id</param>
    /// <param name="msg">Message to show to user</param>
    public static void FinishWork(this ViewModelBase vmBase, string id, string msg = "Task completed")
    {
        using TaskDialog taskDialog = new(Strings.Done);
        taskDialog.CommonButtons = TaskDialogCommonButtons.Close;
        taskDialog.Id = id;
        taskDialog.MainContent = msg;

        vmBase.IsViewEnabled = false;
        taskDialog.Show();
        vmBase.IsViewEnabled = true;
    }
}