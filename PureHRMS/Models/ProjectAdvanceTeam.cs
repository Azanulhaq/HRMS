using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PureHRMS.Models
{
    public class ProjectAdvanceTeam
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string AllowedHours { get; set; }
        public string HoursWorked { get; set; }
        public string AllowedCost { get; set; }
        public string ActualCost { get; set; }
        public string Results { get; set; }
        public string HourRate { get; set; }
    }
}