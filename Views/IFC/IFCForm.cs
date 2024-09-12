﻿using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace VLS.BatchExportNet.Views.IFC
{
    public class IFCForm : IFCExportOptions
    {
        private string _destinationFolder;
        private string _prefix;
        private string _postfix;
        private List<string> _rvtFiles;
        private string _viewName;
        private bool _exportView;

        public string DestinationFolder { get => _destinationFolder; set => _destinationFolder = value; }
        public string Prefix { get => _prefix; set => _prefix = value; }
        public string Postfix { get => _postfix; set => _postfix = value; }
        public List<string> RVTFiles { get => _rvtFiles; set => _rvtFiles = value; }
        public string ViewName { get => _viewName; set => _viewName = value; }
        public bool ExportView { get => _exportView; set => _exportView = value; }
    }
}