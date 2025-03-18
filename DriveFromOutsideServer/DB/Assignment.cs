using Newtonsoft.Json.Linq;

namespace DriveFromOutsideServer.DB
{
    public class Assignment
    {
        public AssignmentType Type { get; set; }
        public JObject Config { get; set; }
    }
    public class EmperorAssignment : Assignment
    {
        public EmperorAssignment(Assignment emperor)
        {
            this.Type = emperor.Type;
            this.Config = emperor.Config;
        }
        public int Id { get; set; }
        public DateTime IssueTime { get; set; }
        public AssignmentStatus Status { get; set; }
        public ICollection<KingAssignment>? Kings { get; set; }
    }
    public class KingAssignment : Assignment
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public DateTime IssueTime { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public AssignmentStatus Status { get; set; }
        public int EmperorId { get; set; }
        public EmperorAssignment Emperor { get; set; }
        public int? KingId { get; set; }
        public KingAssignment? King { get; set; }
    }
}