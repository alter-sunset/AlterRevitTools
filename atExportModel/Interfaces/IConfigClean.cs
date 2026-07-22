using AlterTools.atExportModel.Enums;

namespace AlterTools.atExportModel.Interfaces;

public interface IConfigClean
{
    bool UnloadLinks { get; set; }
    bool RemoveLinkedRvt { get; set; }
    bool RemoveLinkedCad { get; set; }
    bool RemoveOrphanedRooms { get; set; }
#if R22_OR_GREATER
    bool RemoveEmptyWorksets { get; set; }
#endif
    bool Purge { get; set; }
    SheetOptions SheetOptions { get; set; }

    // Remove views enum? or class
    // remove sheets enum or class?
}