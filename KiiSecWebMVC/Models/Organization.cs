namespace KiiSec.Models
{
    public class Organization
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public ICollection<Employee>? Employees { get; set; }
        public ICollection<VisitorsGroup>? VisitorsGroups{ get; set; }
        public ICollection<Visit>? Visits { get; set; }
    }
}
