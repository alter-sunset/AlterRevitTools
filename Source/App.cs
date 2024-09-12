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
            string[] vlsIconPath = { "VLS.png", "VLS_16.png" };
            string[] detachIconPath = { "detach.png", "detach_16.png" };
            string[] migrateIconPath = { "migrate.png", "migrate_16.png" };
            string[] transmitIconPath = { "transmit.png", "transmit_16.png" };
            string[] ifcIconPath = { "ifc.png", "ifc_16.png" };
            string[] nvcIconPath = { "navisworks.png", "navisworks_16.png" };

            RibbonPanel panelIntern = RibbonPanel(a, "Внутренние штуки");
            string[] rvtIconPath = { "rvt.png", "rvt_16.png" };

            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.NWC), "Пакетный экспорт в NWC", nvcIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.IFC), "Пакетный экспорт в IFC", ifcIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.Detach), "Пакетный экспорт отсоединённых моделей", detachIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.Transmit), "Пакетная передача моделей", transmitIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.Migrate), "Пакетная миграция моделей с обновлением связей", migrateIconPath);

            CreateNewPushButton(panelIntern, PushButtonDataWrapper(Forms.Link), "Пакетное добавление Revit ссылок", rvtIconPath);

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;
        private static RibbonPanel RibbonPanel(UIControlledApplication a, string panelName)
        {
            string tabName = "VLS";
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(tabName);
            }
            catch { }
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);
            }
            catch { }
            List<RibbonPanel> panels = a.GetRibbonPanels(tabName);
            foreach (RibbonPanel p in panels.Where(p => p.Name == panelName))
            {
                ribbonPanel = p;
            }
            return ribbonPanel;
        }
        private static void CreateNewPushButton(RibbonPanel ribbonPanel, PushButtonData pushButtonData, string toolTip, string[] iconPath)
        {
            const string BASEPATH = "VLS.BatchExportNet.Resources.";
            BitmapSource bitmap_32 = GetEmbeddedImage(BASEPATH + iconPath[0]);
            BitmapSource bitmap_16 = GetEmbeddedImage(BASEPATH + iconPath[1]);
            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;
            pushButton.ToolTip = toolTip;
            pushButton.Image = bitmap_16;
            pushButton.LargeImage = bitmap_32;
        }
        private PushButtonData PushButtonDataWrapper(Forms forms)
        {
            const string BASE = "VLS.BatchExportNet.Source.";
            string name = "";
            string text = "";
            string className = "";
            switch (forms)
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
        private static BitmapSource GetEmbeddedImage(string name)
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
    public enum Forms
    {
        Detach,
        IFC,
        NWC,
        Migrate,
        Transmit,
        Link
    }
}