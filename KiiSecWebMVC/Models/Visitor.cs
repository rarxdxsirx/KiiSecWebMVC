namespace KiiSec.Models
{
    public class Visitor : User
    {
        public string? VisitorOrganization { get; set; }
        public ICollection<GroupsOfVisitors> GroupsOfVisitors { get; set;}
    }
}
