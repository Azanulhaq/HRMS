using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PureHRMS.Models
{
    public class MyTicketList
    {
        public int ID { get;set;}
        public string Subject { get; set; }
        public string Department { get; set; }
        public bool Read { get; set; }
    }
}