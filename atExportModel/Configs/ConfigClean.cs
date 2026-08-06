using AlterTools.atExportModel.Interfaces;

namespace AlterTools.atExportModel.Configs;

public class ConfigClean : IConfigClean
{
    public bool UnloadLinks { get; set; } = true;
    public bool RemoveLinkedRvt { get; set; } = false;
    public bool RemoveLinkedCad { get; set; } = true;
    public bool RemoveOrphanedRooms { get; set; } = true;
    public bool RemoveEmptyWorksets { get; set; } = false;
    public bool Purge { get; set; } = true;
    public bool RemoveSheets { get; set; } = false;
    public bool RemoveViews { get; set; } = false;
    public ConfigRemoveViews ConfigRemoveViews { get; set; } = new();
}