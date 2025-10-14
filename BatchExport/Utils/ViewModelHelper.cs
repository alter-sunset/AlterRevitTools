using System.Windows;
using AlterTools.BatchExport.Resources;
using AlterTools.BatchExport.Views.Base;
using AlterTools.BatchExport.Views.Detach;
using AlterTools.BatchExport.Views.Link;
using AlterTools.BatchExport.Views.Params;
using AlterTools.BatchExport.Views.Transmit;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.BatchExport.Utils;

internal static class ViewModelHelper
{
    private static string NoFolder => Strings.NoFolder;
    private static string WrongFolder => Strings.WrongFolder;
    private static string CreateFolderError => Strings.CreateFolderError;
    private static string ToHell => Strings.ToHell;
    private static string NoFiles => Strings.NoFiles;
    private static string NoViewName => Strings.NoViewName;
    private static string NoPathMode => Strings.NoPathMode;
    private static string NoMaskPath => Strings.NoMaskPath;
    private static string WrongMask => Strings.WrongMask;
    private static string NoMaskFile => Strings.NoMaskFile;
    private static string NoCsv => Strings.NoCsv;
    private static string NoParameters =>  Strings.NoParameters;

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

    private static bool IsListNotEmpty(this LinkViewModel linkVm) => CheckCondition(linkVm.Entries.Count > 0, NoFiles);

    private static bool IsListNotEmpty(this ViewModelBase vmBase) =>
        CheckCondition(vmBase.ListBoxItems.Count > 0, NoFiles);

    private static bool IsFolderPathOk(this ViewModelBase vmBase)
    {
        string folderPath = vmBase.FolderPath;

        if (string.IsNullOrEmpty(folderPath))
        {
            return CheckCondition(false, NoFolder);
        }

        if (Uri.IsWellFormedUriString(folderPath, UriKind.RelativeOrAbsolute))
        {
            return CheckCondition(false, WrongFolder);
        }

        if (Directory.Exists(folderPath)) return true;

        MessageBoxResult result = MessageBox.Show(CreateFolderError,
            Strings.GoodEvening,
            MessageBoxButton.YesNo);

        if (result is MessageBoxResult.Yes)
        {
            Directory.CreateDirectory(folderPath);
            return true;
        }

        MessageBox.Show(ToHell);
        return false;
    }

    private static bool IsViewNameOk(this ViewModelBaseExtended vmBaseExt)
    {
        return CheckCondition(!vmBaseExt.ExportScopeView
                              || !string.IsNullOrEmpty(vmBaseExt.ViewName), NoViewName);
    }

    private static bool IsViewNameOk(this DetachViewModel detachVm)
    {
        return CheckCondition(!detachVm.CheckForEmptyView
                              || !string.IsNullOrEmpty(detachVm.ViewName), NoViewName);
    }

    private static bool IsRbModeOk(this DetachViewModel detachVm)
    {
        switch (detachVm.RadioButtonMode)
        {
            case 0:
                return CheckCondition(false, NoPathMode);

            case 1:
                return detachVm.IsFolderPathOk();

            case 2:
                if (string.IsNullOrEmpty(detachVm.MaskIn) || string.IsNullOrEmpty(detachVm.MaskOut))
                {
                    return CheckCondition(false, NoMaskPath);
                }

                if (!detachVm.ListBoxItems
                        .Select(item => item.Content)
                        .All(i => i.ToString()
                            !.Contains(detachVm.MaskIn)))
                {
                    return CheckCondition(false, WrongMask);
                }

                break;
        }

        return true;
    }

    private static bool IsMaskNameOk(this DetachViewModel detachVm)
    {
        return CheckCondition(!detachVm.IsToRename
                              || !string.IsNullOrEmpty(detachVm.MaskInName), NoMaskFile);
    }

    private static bool IsCsvPathNotEmpty(this ParamsViewModel paramsVm)
    {
        string csvPath = paramsVm.CsvPath;

        if (string.IsNullOrWhiteSpace(csvPath)
            || Uri.IsWellFormedUriString(csvPath, UriKind.Absolute)
            || !csvPath.EndsWith(".csv"))
        {
            return CheckCondition(false, NoCsv);
        }

        return true;
    }

    private static bool AreThereAnyParameters(this ParamsViewModel paramsVm)
    {
        return CheckCondition(paramsVm.ParametersNames.Length > 0, NoParameters);
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
    public static void Finisher(this ViewModelBase vmBase, string id, string msg = "Task completed")
    {
        TaskDialog taskDialog = new(Strings.Done)
        {
            CommonButtons = TaskDialogCommonButtons.Close,
            Id = id,
            MainContent = msg
        };

        vmBase.IsViewEnabled = false;
        taskDialog.Show();
        vmBase.IsViewEnabled = true;
    }

    /// <returns>Unique files with .rvt extension</returns>
    public static IEnumerable<string> FilterRevitFiles(this IEnumerable<string> files)
    {
        return files.Distinct()
            .Where(file => !string.IsNullOrWhiteSpace(file)
                           && Path.GetExtension(file) == ".rvt");
    }

    public static string RemoveDetach(this string name)
    {
        return name.Replace(Strings.Detached, "");
    }

    public static string[] SplitBySemicolon(this string line)
    {
        return [.. line.Split(';')
            .Select(word => word.Trim())
            .Distinct()
            .Where(word => !string.IsNullOrWhiteSpace(word))];
    }
}