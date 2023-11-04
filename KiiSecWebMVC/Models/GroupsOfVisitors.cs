namespace KiiSec.Models
{
    public class GroupsOfVisitors
    {
        public int VisitorId { get; set; }
        public int GroupId { get; set; }
        public VisitorsGroup VisitorsGroup { get; set; }
        public Visitor Visitor { get; set; }
    }
}
