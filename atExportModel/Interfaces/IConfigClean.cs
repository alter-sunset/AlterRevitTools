using AlterTools.atExportModel.Enums;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigClean
{
    bool UnloadLinks { get; set; } // should i?
    bool RemoveLinkedRvt { get; set; }
    bool RemoveLinkedCad { get; set; }
    bool RemoveOrphanedRooms { get; set; }
#if R22_OR_GREATER
    bool RemoveEmptyWorksets { get; set; }
#endif
    bool Purge { get; set; }
    bool RemoveSheets { get; set; }

    bool RemoveViews { get; set; }
    // add viewTypes (steal from eTransmit)
}