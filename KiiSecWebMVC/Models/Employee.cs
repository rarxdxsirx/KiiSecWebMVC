namespace KiiSec.Models
{
    public class Employee : User
    {
        public int OrganizationId { get; set; }
        public string Department { get; set; }
        public ICollection<EmployeePermissions> EmployeePermissions { get; set; }


        // количество посещений в день/месяц/год с возможностью группировки по подразделениям - в форме таблицы и графика; 
        // список лиц, находящихся на текущий момент на территории организации с группировкой по подразделения. 
        // автоматическое формирование  отчета о количестве посетителей по каждому подразделению за каждые 3 часа и сохранение на ПК сотрудника общего отдела 
        //public string[] AviablePeriods(TimeSpan[] startTimes, int[] durations, TimeSpan startShift, TimeSpan endShift, int consultationTime)
        //{
        //    return new string[] {""};
        //}
    }
}
