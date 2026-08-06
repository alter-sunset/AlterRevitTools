using AlterTools.atExportModel.Configs;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigClean
{
    bool UnloadLinks { get; set; } // should i?
    bool RemoveLinkedRvt { get; set; }
    bool RemoveLinkedCad { get; set; }
    bool RemoveOrphanedRooms { get; set; }
    bool RemoveEmptyWorksets { get; set; }
    bool Purge { get; set; }
    bool RemoveSheets { get; set; }
    bool RemoveViews { get; set; }
    ConfigRemoveViews ConfigRemoveViews { get; set; }
}