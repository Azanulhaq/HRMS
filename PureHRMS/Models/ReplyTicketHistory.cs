using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PureHRMS.Models
{
    public class ReplyTicketHistory
    {
       public string Text { get; set; }
        public string CreatedBy { get; set; }
        public string AttachedFile { get; set; }
        public DateTime? Date { get; set;}
    }
}