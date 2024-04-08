using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PureHRMS.Models
{
    public class MonthlySalGenerator
    {
        public int Euid { get; set; }
        public string EmpID { get; set; }
        public string Name { get; set; }
        public int NetSalaryAmount { get; set; }
        public decimal WorkingDays { get; set; }
        public decimal DaysWorked { get; set; }
        public int NetPayAmt { get; set; }
        public int MoreDeductions { get; set; }
        public string MoreDeductionsNotes { get; set; }
        public int MoreAddition { get; set; }
        public string MoreAdditionNotes { get; set; }
        public int FinalSalAmt { get; set; }
    }
}