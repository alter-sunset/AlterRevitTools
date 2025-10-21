using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.BatchExport.Utils;

internal static class ViewHelper
{
    private static Window _myForm;
    
    internal static void ShowForm(Func<Window> window)
    {
        CloseCurrentForm();

        try
        {
            _myForm = window();
            _myForm.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private static void CloseCurrentForm()
    {
        if (_myForm is null) return;

        _myForm.Close();
        _myForm = null;
    }
}