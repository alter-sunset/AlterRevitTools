using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Panel = System.Tuple<Autodesk.Revit.UI.RibbonPanel, string>;

namespace VLS.BatchExportNet.Source
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiApp)
        {
            const string TAB_NAME = "VLS";

            //Create default tab 
            try
            {
                uiApp.CreateRibbonTab(TAB_NAME);
            }
            catch { }

            //Get buttons to create from json config
            ButtonContext[] buttons = ButtonContext.GetButtonsContext();

            //Create panels from config
            IEnumerable<Panel> panels = buttons
                .Select(button => button.Panel)
                .Distinct()
                .Select(panelName => new Panel(GetRibbonPanel(uiApp, TAB_NAME, panelName), panelName));

            //Create buttons from config
            foreach (ButtonContext button in buttons)
            {
                CreateButton(button, panels);
            }

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;
        private static RibbonPanel GetRibbonPanel(UIControlledApplication uiApp, string tabName, string panelName)
        {
            try
            {
                RibbonPanel panel = uiApp.CreateRibbonPanel(tabName, panelName);
            }
            catch { }
            return uiApp.GetRibbonPanels(tabName).FirstOrDefault(p => p.Name == panelName);
        }
        private static void CreateButton(ButtonContext button, IEnumerable<Panel> panels)
        {
            RibbonPanel ribbonPanel = panels.First(e => e.Item2 == button.Panel).Item1;
            try
            {
                ribbonPanel.AddItem(button.GetPushButtonData());
            }
            catch { }
        }
    }
}