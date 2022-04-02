namespace Vasil_Stoyanov_employees.Services.Models
{
    using System.Collections.Generic;

    public class BestPairServiceModel
    {
        public BestPairServiceModel() => this.ProjectsAndDaysWorked = new Dictionary<int, int>();

        public int EmpOneId { get; set; }

        public int EmpTwoId { get; set; }

        public int TotalDaysWorked { get; set; }

        public IDictionary<int, int> ProjectsAndDaysWorked { get; set; }
    }
}
