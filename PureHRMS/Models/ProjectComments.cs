using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PureHRMS.Models
{
    public class ProjectComments
    {
        public DateTime?  Date { get; set; }
        public string Hours { get; set; }
        public string Comments { get; set; }
        public string Employee { get; set; } 
    }
}