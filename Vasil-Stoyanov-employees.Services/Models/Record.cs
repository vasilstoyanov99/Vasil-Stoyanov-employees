namespace Vasil_Stoyanov_employees.Services.Models
{
    using System;

    public class Record
    {
        public int EmpId { get; set; }

        public int ProjectId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
