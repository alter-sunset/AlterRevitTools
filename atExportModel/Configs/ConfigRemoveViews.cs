namespace AlterTools.atExportModel.Configs;

public class ConfigRemoveViews
{
    public bool? All { get; set; } = false;
    public bool ThreeDViews { get; set; }
    public bool AreaPlans { get; set; }
    public bool CeilingPlans { get; set; }
    public bool Details { get; set; }
    public bool DraftingViews { get; set; }
    public bool Elevations { get; set; }
    public bool FloorPlans { get; set; }
    public bool ColumnSchedules { get; set; }
    public bool Legends { get; set; }
    public bool Renderings { get; set; }
    public bool Schedules { get; set; }
    public bool Sections { get; set; }
    public bool EngineeringPlans { get; set; }
    public bool Walkthroughs { get; set; }
}