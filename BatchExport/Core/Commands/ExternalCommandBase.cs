using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace AlterTools.BatchExport.Core.Commands;

public abstract class ExternalCommandBase : IExternalCommand
{
    private protected virtual Func<Window> WindowFactory => null;
    private protected static ExternalCommandData CommandData;
    private static Window _myForm;
    
    public Result Execute(ExternalCommandData commandData, ref string msg, ElementSet elements)
    {
        CommandData =  commandData;
        
        try
        {
            // If there is an already opened window - close and nullify it
            if (_myForm is not null)
            {
                _myForm.Close();
                _myForm = null;
            }
            
            // Assign new window handler obtained from derived class
            _myForm = WindowFactory();
            _myForm.Show();
            
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            MessageBox.Show(ex.Message);
            return Result.Failed;
        }
    }
}