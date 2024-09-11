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
        // class instance
        private static App _thisApp;

        // ModelessForm instance
        private static NWCExportUi _mMyFormNWC;
        private static IFCExportUi _mMyFormIFC;
        private static DetachModelsUi _mMyFormDetach;
        private static TransmitModelsUi _mMyFormTransmit;
        private static MigrateModelsUi _mMyFormMigrate;
        private static LinkModelsUi _mMyFormLink;

        public Result OnStartup(UIControlledApplication a)
        {
            _mMyFormNWC = null; // no dialog needed yet; the command will bring it
            _mMyFormIFC = null;
            _mMyFormDetach = null;
            _thisApp = this; // static access to this application instance

            // Method to add Tab and Panel 
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

        /// <summary>
        /// What to do when the application is shut down.
        /// </summary>
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;

        /// <summary>
        /// This is the method which launches the WPF window, and injects any methods that are
        /// wrapped by ExternalEventHandlers. This can be done in a number of different ways, and
        /// implementation will differ based on how the WPF is set up.
        /// </summary>
        /// <param name="uiapp">The Revit UIApplication within the add-in will operate.</param>
        public static void ShowFormNWC(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyFormNWC != null && _mMyFormNWC == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerNWCExportUiArg evUi = new();
            EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg = new();

            // The dialog becomes the owner responsible for disposing the objects given to it.
            _mMyFormNWC = new NWCExportUi(uiapp, evUi, eventHandlerNWCExportBatchUiArg) { Height = 900, Width = 800 };
            _mMyFormNWC.Show();
        }
        public static void ShowFormIFC(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyFormIFC != null && _mMyFormIFC == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerIFCExportUiArg evUi = new();

            // The dialog becomes the owner responsible for disposing the objects given to it.
            _mMyFormIFC = new IFCExportUi(uiapp, evUi) { Height = 700, Width = 800 };
            _mMyFormIFC.Show();
        }
        public static void ShowFormDetach(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyFormDetach != null && _mMyFormDetach == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerDetachModelsUiArg evUi = new();

            // The dialog becomes the owner responsible for disposing the objects given to it.

            try
            {
                _mMyFormDetach = new DetachModelsUi(uiapp, evUi) { Height = 600, Width = 800 };
                _mMyFormDetach.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void ShowFormTransmit(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyFormTransmit != null && _mMyFormTransmit == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerTransmitModelsUiArg evUi = new();

            // The dialog becomes the owner responsible for disposing the objects given to it.

            try
            {
                _mMyFormTransmit = new TransmitModelsUi(uiapp, evUi) { Height = 500, Width = 800 };
                _mMyFormTransmit.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void ShowFormMigrate(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyFormMigrate != null && _mMyFormMigrate == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerMigrateModelsUiArg evUi = new();

            // The dialog becomes the owner responsible for disposing the objects given to it.

            try
            {
                _mMyFormMigrate = new MigrateModelsUi(uiapp, evUi) { Height = 200, Width = 600 };
                _mMyFormMigrate.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public static void ShowFormLink(UIApplication uiapp)
        {
            // If we do not have a dialog yet, create and show it
            if (_mMyFormLink != null && _mMyFormLink == null) return;
            //EXTERNAL EVENTS WITH ARGUMENTS
            EventHandlerLinkModelsUiArg evUi = new();
            // The dialog becomes the owner responsible for disposing the objects given to it.
            try
            {
                _mMyFormLink = new LinkModelsUi(uiapp, evUi) { Height = 500, Width = 800 };
                _mMyFormLink.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #region Ribbon Panel
        public RibbonPanel RibbonPanel(UIControlledApplication a, string tabName)
        {
            string tab = "VLS"; // Tab name
            // Empty ribbon panel 
            RibbonPanel ribbonPanel = null;
            // Try to create ribbon tab. 
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch
            {
            }

            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, tabName);
            }
            catch
            {
            }

            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == tabName))
            {
                ribbonPanel = p;
            }

            //return panel 
            return ribbonPanel;
        }
        #endregion
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
}