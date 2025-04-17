using Autodesk.Revit.UI;
using System.Linq;
using System.Collections.Generic;
using Panel = System.Tuple<Autodesk.Revit.UI.RibbonPanel, string>;

namespace AlterTools.BatchExport.Core
{//TODO: Add RevitServerViewer
    public class App : IExternalApplication
    {
        private const string TabName = "AlterTools";
        private Panel[] _panels;

        public Result OnStartup(UIControlledApplication uiApp)
        {
            try
            {
                uiApp.CreateRibbonTab(TabName);
            }
            catch
            {
                // ignored
            }

            //Get buttons to create from json config
            List<ButtonContext> buttons = ButtonContext.GetButtonsContext();

            //Create panels from config
            _panels = buttons.Select(button => button.Panel)
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
                uiApp.CreateRibbonPanel(TabName, panelName);
            }
            catch
            {
                // ignored
            }

            return uiApp.GetRibbonPanels(TabName)
                        .FirstOrDefault(panel => panel.Name == panelName);
        }

        private void CreateButton(ButtonContext button)
        {
            RibbonPanel ribbonPanel = _panels.First(panel => panel.Item2 == button.Panel).Item1;

            try
            {
                ribbonPanel.AddItem(button.GetPushButtonData());
            }
            catch
            {
                // ignored
            }
        }
    }
}