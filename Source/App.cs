using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace VLS.BatchExportNet.Source
{
    public class App : IExternalApplication
    {
        private string _thisAssemblyPath;

        public Result OnStartup(UIControlledApplication a)
        {
            _thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            RibbonPanel panelExtern = RibbonPanel(a, "Пакетный экспорт");
            RibbonPanel panelIntern = RibbonPanel(a, "Внутренние штуки");

            //certainly not the best way to add multiple buttons, still looking for smthng better
            CreateNewPushButton(panelExtern, Forms.NWC);
            CreateNewPushButton(panelExtern, Forms.IFC);
            CreateNewPushButton(panelExtern, Forms.Detach);
            CreateNewPushButton(panelExtern, Forms.Transmit);
            CreateNewPushButton(panelExtern, Forms.Migrate);

            CreateNewPushButton(panelIntern, Forms.Link);

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;
        private static RibbonPanel RibbonPanel(UIControlledApplication a, string panelName)
        {
            const string TAB_NAME = "VLS";
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(TAB_NAME);
            }
            catch { }
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(TAB_NAME, panelName);
            }
            catch { }
            List<RibbonPanel> panels = a.GetRibbonPanels(TAB_NAME);
            foreach (RibbonPanel p in panels.Where(p => p.Name == panelName))
            {
                ribbonPanel = p;
            }
            return ribbonPanel;
        }
        private void CreateNewPushButton(RibbonPanel ribbonPanel, Forms form)
        {
            const string BASEPATH = "VLS.BatchExportNet.Resources.";
            ButtonContext buttonContext = GetButtonContext(form);
            BitmapSource bitmap_32 = GetEmbeddedImage(BASEPATH + buttonContext.LargeImage);
            BitmapSource bitmap_16 = GetEmbeddedImage(BASEPATH + buttonContext.SmallImage);
            PushButton pushButton = ribbonPanel.AddItem(PushButtonDataWrapper(form)) as PushButton;
            pushButton.ToolTip = buttonContext.ToolTip;
            pushButton.Image = bitmap_16;
            pushButton.LargeImage = bitmap_32;
        }
        private PushButtonData PushButtonDataWrapper(Forms form)
        {
            const string BASE = "VLS.BatchExportNet.Source.";
            string name = "";
            string text = "";
            string className = "";
            switch (form)
            {
                case Forms.Detach:
                    name = "Экспорт отсоединённых моделей";
                    text = "Экспорт\nотсоединённых\nмоделей";
                    className = BASE + "ExportModelsDetached";
                    break;

                case Forms.IFC:
                    name = "Экспорт IFCHelper";
                    text = "Экспорт\nIFC";
                    className = BASE + "ExportModelsToIFC";
                    break;

                case Forms.NWC:
                    name = "Экспорт NWCHelper";
                    text = "Экспорт\nNWC";
                    className = BASE + "ExportModelsToNWC";
                    break;

                case Forms.Migrate:
                    name = "Миграция моделей";
                    text = "Миграция\nмоделей";
                    className = BASE + "MigrateModels";
                    break;

                case Forms.Transmit:
                    name = "Передача моделей";
                    text = "Передача\nмоделей";
                    className = BASE + "ExportModelsTransmitted";
                    break;

                case Forms.Link:
                    return new PushButtonData(
                            "Batch add Revit links",
                            "Batch add\nRevit Links",
                            _thisAssemblyPath,
                            BASE + "LinkModels");
            }
            return new PushButtonData(name, text, _thisAssemblyPath, className)
            {
                AvailabilityClassName = BASE + "CommandAvailability"
            };
        }
        private static ButtonContext GetButtonContext(Forms form)
        {
            ButtonContext buttonContext = new();
            switch (form)
            {
                case Forms.Detach:
                    buttonContext.ToolTip = "Пакетный экспорт отсоединённых моделей";
                    buttonContext.SmallImage = "detach_16.png";
                    buttonContext.LargeImage = "detach.png";
                    return buttonContext;

                case Forms.IFC:
                    buttonContext.ToolTip = "Пакетный экспорт в IFC";
                    buttonContext.SmallImage = "ifc_16.png";
                    buttonContext.LargeImage = "ifc.png";
                    return buttonContext;

                case Forms.NWC:
                    buttonContext.ToolTip = "Пакетный экспорт в NWC";
                    buttonContext.SmallImage = "navisworks_16.png";
                    buttonContext.LargeImage = "navisworks.png";
                    return buttonContext;

                case Forms.Migrate:
                    buttonContext.ToolTip = "Пакетная миграция моделей с обновлением связей";
                    buttonContext.SmallImage = "migrate_16.png";
                    buttonContext.LargeImage = "migrate.png";
                    return buttonContext;

                case Forms.Transmit:
                    buttonContext.ToolTip = "Пакетная передача моделей";
                    buttonContext.SmallImage = "transmit_16.png";
                    buttonContext.LargeImage = "transmit.png";
                    return buttonContext;

                case Forms.Link:
                    buttonContext.ToolTip = "Пакетное добавление Revit ссылок";
                    buttonContext.SmallImage = "rvt_16.png";
                    buttonContext.LargeImage = "rvt.png";
                    return buttonContext;

                case Forms.VLS:
                    buttonContext.ToolTip = "Велесстрой";
                    buttonContext.SmallImage = "VLS_16.png";
                    buttonContext.LargeImage = "VLS.png";
                    return buttonContext;
            }
            return buttonContext;
        }
        private static BitmapFrame GetEmbeddedImage(string name)
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream(name);
                return BitmapFrame.Create(s);
            }
            catch
            {
                return null;
            }
        }
    }
}