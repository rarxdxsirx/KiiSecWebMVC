namespace KiiSec.Models
{
    public class VisitOfVisitor
    {
        public int VisitorId { get; set; }
        public int VisitId { get; set; }
        public Visit Visit { get; set; }
        public Visitor Visitor { get; set; }
    }
}
