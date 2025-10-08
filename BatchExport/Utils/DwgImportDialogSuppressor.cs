using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;

namespace AlterTools.BatchExport.Utils;

public class DwgImportDialogSuppressor : IDisposable
{
    private const string TargetTitle = "Revit";
    private const int CheckIntervalMs = 300;
    private const uint WmClose = 0x0010;
    private static string TargetMessage => Resources.Strings.Const_DwgImportDialog;

    private Thread _watcherThread;
    private bool _running;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

    public DwgImportDialogSuppressor()
    {
        _running = true;
        _watcherThread = new Thread(WatchLoop)
        {
            IsBackground = true
        };
        _watcherThread.Start();
    }

    private void WatchLoop()
    {
        while (_running)
        {
            Thread.Sleep(CheckIntervalMs);
            
            try
            {
                IntPtr hWnd = FindWindow(null, TargetTitle);
                if (hWnd == IntPtr.Zero) continue;
                
                AutomationElement dialog = AutomationElement.FromHandle(hWnd);
                if (dialog is null) continue;
                
                AutomationElement textElement = dialog.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                
                string msg = textElement?.Current.Name;
                if (msg is null || !msg.Contains(TargetMessage)) continue;
                
                SendMessage(hWnd, WmClose, 0, 0);
            }
            catch
            {
                // ignored
            }
        }
    }

    public void Dispose()
    {
        _running = false;
        if (_watcherThread is not { IsAlive: true }) return;
        
        _watcherThread.Join();
        _watcherThread = null;
    }
}