using Autodesk.Revit.UI;
using System.Linq;
using System.Collections.Generic;
using Panel = System.Tuple<Autodesk.Revit.UI.RibbonPanel, string>;

namespace AlterTools.BatchExport.Source
{//TODO: Add RevitServerViewer
    public class App : IExternalApplication
    {
        private Panel[] Panels;
        private const string TAB_NAME = "AlterTools";
        public Result OnStartup(UIControlledApplication uiApp)
        {
            try
            {
                uiApp.CreateRibbonTab(TAB_NAME);
            }
            catch { }

            //Get buttons to create from json config
            List<ButtonContext> buttons = ButtonContext.GetButtonsContext();

            //Create panels from config
            Panels = buttons
                .Select(button => button.Panel)
                .Distinct()
                .Select(panelName => new Panel(GetRibbonPanel(uiApp, panelName), panelName))
                .ToArray();

            buttons.ForEach(CreateButton);

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;
        private static RibbonPanel GetRibbonPanel(UIControlledApplication uiApp, string panelName)
        {
            try
            {
                RibbonPanel panel = uiApp.CreateRibbonPanel(TAB_NAME, panelName);
            }
            catch { }
            return uiApp.GetRibbonPanels(TAB_NAME).FirstOrDefault(p => p.Name == panelName);
        }
        private void CreateButton(ButtonContext button)
        {
            RibbonPanel ribbonPanel = Panels.First(e => e.Item2 == button.Panel).Item1;
            try
            {
                ribbonPanel.AddItem(button.GetPushButtonData());
            }
            catch { }
        }
    }
}