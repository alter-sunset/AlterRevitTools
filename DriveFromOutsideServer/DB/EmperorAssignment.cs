using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DriveFromOutsideServer.DB
{
    public class Emperor
    {
        public AssignmentType Type { get; set; }
        public JObject Config { get; set; }
    }
    public class EmperorAssignment : Emperor
    {
        public EmperorAssignment(Emperor emperor)
        {
            this.Type = emperor.Type;
            this.Config = emperor.Config;
        }
        public int Id { get; set; }
        public DateTime IssueTime { get; set; }
        public AssignmentStatus Status { get; set; }
        public ICollection<KingAssignment>? Kings { get; set; }
    }
}