namespace KiiSec.Models
{
    public class Organization
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public ICollection<Employee>? Employees { get; set; }
        public ICollection<Visit>? Visits { get; set; }
    }
}
