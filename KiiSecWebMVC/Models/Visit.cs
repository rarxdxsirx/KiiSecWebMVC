namespace KiiSec.Models
{
    public class Visit
    {
        public int ID { get; set; }
        public int VisitorsGroupID { get; set; }
        public int OrganizationID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime? VisitDate { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
        public string VisitPurpose { get; set; }
        public int VisitStatusID { get; set; }
    }
}
