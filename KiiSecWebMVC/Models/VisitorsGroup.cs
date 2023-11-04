namespace KiiSec.Models
{
    public class VisitorsGroup
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public ICollection<GroupsOfVisitors> GroupsOfVisitors { get; set; }
    }
}
