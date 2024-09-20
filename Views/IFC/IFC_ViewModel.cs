using Autodesk.Revit.DB;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.IFC
{
    public class IFC_ViewModel : ViewModelBase_Extended
    {
        public IFC_ViewModel(EventHandlerIFCExportVMArg eventHandlerIFCExportUiArg)
        {
            EventHandlerBaseVMArgs = eventHandlerIFCExportUiArg;
            Dictionary<HelpMessages, string> help = Help.GetHelpMessages();
            string _helpMessage = help.GetValueOrDefault(HelpMessages.IFCTitle) +
                "\n" + help.GetValueOrDefault(HelpMessages.Load) +
                "\n" + help.GetValueOrDefault(HelpMessages.Folder) +
                "\n" + help.GetValueOrDefault(HelpMessages.Naming) +
                "\n" + help.GetValueOrDefault(HelpMessages.Config) +
                "\n" + help.GetValueOrDefault(HelpMessages.Start);
            HelpMessage = _helpMessage;
        }

        private string _mapping = "";
        public string Mapping
        {
            get => _mapping;
            set
            {
                _mapping = value;
                OnPropertyChanged("Mapping");
            }
        }

        private RelayCommand _loadMapping;
        public RelayCommand LoadMapping
        {
            get
            {
                return _loadMapping ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Multiselect = false,
                        DefaultExt = ".txt",
                        Filter = "Текстовый файл (.txt)|*.txt"
                    };

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        try
                        {
                            Mapping = openFileDialog.FileName;
                        }
                        catch
                        {
                            MessageBox.Show("Неверная схема файла");
                        }
                    }
                });
            }
        }

        private bool _exportBaseQuantities = false;
        public bool ExportBaseQuantities
        {
            get => _exportBaseQuantities;
            set
            {
                _exportBaseQuantities = value;
                OnPropertyChanged("ExportBaseQuantities");
            }
        }

        private bool _wallAndColumnSplitting = false;
        public bool WallAndColumnSplitting
        {
            get => _wallAndColumnSplitting;
            set
            {
                _wallAndColumnSplitting = value;
                OnPropertyChanged("WallAndColumnSplitting");
            }
        }

        private RelayCommand _loadListCommand;
        public override RelayCommand LoadListCommand
        {
            get
            {
                return _loadListCommand ??= new RelayCommand(obj =>
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Multiselect = false,
                        DefaultExt = ".json",
                        Filter = "Файл JSON (.json)|*.json"
                    };

                    DialogResult result = openFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        using FileStream file = File.OpenRead(openFileDialog.FileName);
                        try
                        {
                            JsonSerializerOptions options = new()
                            {
                                WriteIndented = true
                            };
                            IFCForm form = JsonSerializer.Deserialize<IFCForm>(file, options);
                            IFCFormDeserilaizer(form);
                            form.Dispose();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                        }
                    }
                });
            }
        }
        private void IFCFormDeserilaizer(IFCForm form)
        {
            FolderPath = form.DestinationFolder;
            NamePrefix = form.NamePrefix;
            NamePostfix = form.NamePostfix;
            WorksetPrefix = form.WorksetPrefix;
            Mapping = form.FamilyMappingFile;
            ExportBaseQuantities = form.ExportBaseQuantities;
            SelectedVersion = _ifcVersions.FirstOrDefault(e => e.Key == form.FileVersion);
            WallAndColumnSplitting = form.WallAndColumnSplitting;
            ExportScopeView = form.ExportView;
            ViewName = form.ViewName;
            SelectedLevel = _spaceBoundaryLevels.FirstOrDefault(e => e.Key == form.SpaceBoundaryLevel);
            ListBoxItems.Clear();
            foreach (string file in form.RVTFiles)
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                ListBoxItem listBoxItem = new() { Content = file, Background = Brushes.White };
                if (!ListBoxItems.Any(cont => cont.Content.ToString() == file)
                    || file.EndsWith(".rvt", true, System.Globalization.CultureInfo.CurrentCulture))
                {
                    ListBoxItems.Add(listBoxItem);
                }
            }
        }

        private RelayCommand _saveListCommand;
        public override RelayCommand SaveListCommand
        {
            get
            {
                return _saveListCommand ??= new RelayCommand(obj =>
                {
                    IFCForm form = IFCFormSerializer();

                    SaveFileDialog saveFileDialog = new()
                    {
                        FileName = "ConfigBatchExportIFC",
                        DefaultExt = ".json",
                        Filter = "Файл JSON (.json)|*.json"
                    };

                    DialogResult result = saveFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        string fileName = saveFileDialog.FileName;
                        File.Delete(fileName);
                        try
                        {
                            JsonSerializerOptions options = new()
                            {
                                WriteIndented = true
                            };
                            File.WriteAllText(fileName, JsonSerializer.Serialize(form, options));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Неверная схема файла\n{ex.Message}");
                        }
                    }

                    form.Dispose();
                });
            }
        }
        private IFCForm IFCFormSerializer() => new()
        {
            ExportBaseQuantities = ExportBaseQuantities,
            FamilyMappingFile = Mapping,
            FileVersion = SelectedVersion.Key,
            SpaceBoundaryLevel = SelectedLevel.Key,
            WallAndColumnSplitting = WallAndColumnSplitting,
            DestinationFolder = FolderPath,
            NamePrefix = NamePrefix,
            NamePostfix = NamePostfix,
            WorksetPrefix = WorksetPrefix,
            ExportView = ExportScopeView,
            ViewName = ViewName,

            RVTFiles = ListBoxItems
                .Select(cont => cont.Content.ToString())
                .ToList()
        };

        private readonly Dictionary<IFCVersion, string> _ifcVersions
            = IFC_Context.IFCVersions;
        public Dictionary<IFCVersion, string> IFCVersions
        {
            get => _ifcVersions;
        }
        private KeyValuePair<IFCVersion, string> _selectedVersion
            = IFC_Context.IFCVersions.FirstOrDefault(e => e.Key == IFCVersion.Default);
        public KeyValuePair<IFCVersion, string> SelectedVersion
        {
            get => _selectedVersion;
            set
            {
                _selectedVersion = value;
                OnPropertyChanged("SelectedVersion");
            }
        }

        private readonly Dictionary<int, string> _spaceBoundaryLevels
            = IFC_Context.SpaceBoundaryLevels;
        public Dictionary<int, string> SpaceBoundaryLevels
        {
            get => _spaceBoundaryLevels;
        }
        private KeyValuePair<int, string> _selectedLevel
            = IFC_Context.SpaceBoundaryLevels.FirstOrDefault(e => e.Key == 1);
        public KeyValuePair<int, string> SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                OnPropertyChanged("SelectedLevel");
            }
        }
    }
}