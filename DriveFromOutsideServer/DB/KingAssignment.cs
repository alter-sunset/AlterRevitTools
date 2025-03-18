using Newtonsoft.Json.Linq;

namespace DriveFromOutsideServer.DB
{
    public class KingAssignment
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public AssignmentType Type { get; set; }
        public DateTime IssueTime { get; set; }
        public JObject Config { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public AssignmentStatus Status { get; set; }
        public int EmperorId { get; set; }
        public EmperorAssignment Emperor { get; set; }
        public int? KingId { get; set; } //Possible Parent
        public KingAssignment? King { get; set; } //Possible Parent
    }
}