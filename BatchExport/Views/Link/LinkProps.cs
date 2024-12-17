﻿using Autodesk.Revit.DB;

namespace VLS.BatchExport.Views.Link
{
    public class LinkProps
    {
        public LinkProps(WorksetTable table, bool setWorksetId)
        {
            WorksetTable = table;
            SetWorksetId = setWorksetId;
        }
        public WorksetTable WorksetTable { get; set; }
        public bool SetWorksetId { get; set; }
    }
}