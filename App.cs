using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Media.Imaging;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.NWC;
using VLS.BatchExportNet.IFC;
using VLS.BatchExportNet.Detach;
using VLS.BatchExportNet.Transmit;
using VLS.BatchExportNet.Migrate;
using VLS.BatchExportNet.Link;
using System.IO;

namespace VLS.BatchExportNet
{
    /// <summary>
    /// This is the main class which defines the Application, and inherits from Revit's
    /// IExternalApplication class.
    /// </summary>
    public class App : IExternalApplication
    {
        private static Window _myForm;

        public Result OnStartup(UIControlledApplication a)
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            RibbonPanel panel = RibbonPanel(a, "Пакетный экспорт");
            string[] applIconPath = { "VLS.BatchExportNet.Resources.VLS.png", "VLS.BatchExportNet.Resources.VLS_16.png" };
            string[] ifcIconPath = { "VLS.BatchExportNet.Resources.ifc.png", "VLS.BatchExportNet.Resources.ifc_16.png" };
            string[] navisIconPath = { "VLS.BatchExportNet.Resources.navisworks.png", "VLS.BatchExportNet.Resources.navisworks_16.png" };

            RibbonPanel panelIntern = RibbonPanel(a, "Внутренние штуки");
            string[] rvtIconPath = { "VLS.BatchExportNet.Resources.rvt.png", "VLS.BatchExportNet.Resources.rvt_16.png" };

            PushButtonData exportModelsToNWCButtonData = new PushButtonData(
                   "Экспорт NWC",
                   "Экспорт\nNWC",
                   thisAssemblyPath,
                   "VLS.BatchExportNet.NWC.ExportModelsToNWC")
            {
                AvailabilityClassName = "VLS.BatchExportNet.NWC.ExportModelsToNWCCommand_Availability"
            };

            string exportModelsToNWCToolTip = "Пакетный экспорт в NWC";
            CreateNewPushButton(panel, exportModelsToNWCButtonData, exportModelsToNWCToolTip, navisIconPath);

            PushButtonData exportModelsDetachedButtonData = new PushButtonData(
                   "Экспорт отсоединённых моделей",
                   "Экспорт\nотсоединённых\nмоделей",
                   thisAssemblyPath,
                   "VLS.BatchExportNet.Detach.ExportModelsDetached")
            {
                AvailabilityClassName = "VLS.BatchExportNet.Detach.ExportModelsDetachedCommand_Availability"
            };

            string exportModelsDetachedToolTip = "Пакетный экспорт отсоединённых моделей";
            CreateNewPushButton(panel, exportModelsDetachedButtonData, exportModelsDetachedToolTip, applIconPath);

            PushButtonData transmitModelsButtonData = new PushButtonData(
                   "Передача моделей",
                   "Передача",
                   thisAssemblyPath,
                   "VLS.BatchExportNet.Transmit.ExportModelsTransmitted")
            {
                AvailabilityClassName = "VLS.BatchExportNet.Transmit.ExportModelsTransmittedCommand_Availability"
            };

            string transmitModelsToolTip = "Пакетная передача моделей";
            CreateNewPushButton(panel, transmitModelsButtonData, transmitModelsToolTip, applIconPath);

            PushButtonData migrateModelsButtonData = new PushButtonData(
                   "Миграция моделей",
                   "Миграция\nмоделей",
                   thisAssemblyPath,
                   "VLS.BatchExportNet.Migrate.MigrateModels")
            {
                AvailabilityClassName = "VLS.BatchExportNet.Migrate.MigrateModelsCommand_Availability"
            };

            string migrateModelsToolTip = "Пакетная миграция моделей с обновлением связей";
            CreateNewPushButton(panel, migrateModelsButtonData, migrateModelsToolTip, applIconPath);

            PushButtonData exportModelsToIFCButtonData = new PushButtonData(
                   "Экспорт IFC",
                   "Экспорт\nIFC",
                   thisAssemblyPath,
                   "VLS.BatchExportNet.IFC.ExportModelsToIFC")
            {
                AvailabilityClassName = "VLS.BatchExportNet.IFC.ExportModelsToIFCCommand_Availability"
            };

            string exportModelsToIFCToolTip = "Пакетный экспорт в IFC";
            CreateNewPushButton(panel, exportModelsToIFCButtonData, exportModelsToIFCToolTip, ifcIconPath);

            PushButtonData linkModelsButtonData = new PushButtonData(
                   "Batch Revit links",
                   "Batch\nRevit Links",
                   thisAssemblyPath,
                   "VLS.BatchExportNet.Link.LinkModels");

            string linkModelsToolTip = "Пакетное добавление Revit ссылок";
            CreateNewPushButton(panelIntern, linkModelsButtonData, linkModelsToolTip, rvtIconPath);

            return Result.Succeeded;
        }
        static void CreateNewPushButton(RibbonPanel ribbonPanel, PushButtonData pushButtonData, string toolTip, string[] iconPath)
        {
            BitmapSource bitmap_32 = GetEmbeddedImage(iconPath[0]);
            BitmapSource bitmap_16 = GetEmbeddedImage(iconPath[1]);
            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;
            pushButton.ToolTip = toolTip;
            pushButton.Image = bitmap_16;
            pushButton.LargeImage = bitmap_32;
        }
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;

        public static void ShowForm(UIApplication uiapp, Forms form)
        {
            if (_myForm != null && _myForm == null) return;

            try
            {
                switch (form)
                {
                    case Forms.Detach:
                        EventHandlerDetachModelsUiArg evDetachUi = new();
                        _myForm = new DetachModelsUi(uiapp, evDetachUi) { Height = 600, Width = 800 };
                        break;
                    case Forms.IFC:
                        EventHandlerIFCExportUiArg evIFCUi = new();
                        _myForm = new IFCExportUi(uiapp, evIFCUi) { Height = 700, Width = 800 };
                        break;
                    case Forms.NWC:
                        EventHandlerNWCExportUiArg evNWCUi = new();
                        EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg = new();
                        _myForm = new NWCExportUi(uiapp, evNWCUi, eventHandlerNWCExportBatchUiArg) { Height = 900, Width = 800 };
                        break;
                    case Forms.Migrate:
                        EventHandlerMigrateModelsUiArg evMigrateUi = new();
                        _myForm = new MigrateModelsUi(uiapp, evMigrateUi) { Height = 200, Width = 600 };
                        break;
                    case Forms.Transmit:
                        EventHandlerTransmitModelsUiArg evTransmitUi = new();
                        _myForm = new TransmitModelsUi(uiapp, evTransmitUi) { Height = 500, Width = 800 };
                        break;
                    case Forms.Link:
                        EventHandlerLinkModelsUiArg evLinkUi = new();
                        _myForm = new LinkModelsUi(uiapp, evLinkUi) { Height = 500, Width = 800 };
                        break;
                }
                _myForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static RibbonPanel RibbonPanel(UIControlledApplication a, string tabName)
        {
            string tab = "VLS";
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch { }
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, tabName);
            }
            catch { }
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == tabName))
            {
                ribbonPanel = p;
            }
            return ribbonPanel;
        }
        public static BitmapSource GetEmbeddedImage(string name)
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