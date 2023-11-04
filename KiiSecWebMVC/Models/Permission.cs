namespace KiiSec.Models 
{
    public class Permission
    {
        public int ID { get; set; }
        public string permission { get; set; }
        public ICollection<EmployeePermissions> EmployeePermissions { get; set;}
    }
}