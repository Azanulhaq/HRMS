using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
    public class tblMonthlySalCustom
    {
        public int MSId { get; set; }
        public Nullable<int> refEid { get; set; }
        public Nullable<int> NetSalAmt { get; set; }
        public Nullable<decimal> WorkingDays { get; set; }
        public Nullable<decimal> DaysWorked { get; set; }
        public Nullable<int> NetPaySalAmt { get; set; }
        public Nullable<int> MoreDeduction { get; set; }
        public string MoreDeductionNotes { get; set; }
        public Nullable<int> MoreAddition { get; set; }
        public string MoreAdditionNotes { get; set; }
        public Nullable<int> FinalSalAmt { get; set; }
        public Nullable<int> ForMonth { get; set; }
        public Nullable<int> ForYear { get; set; }
        public Nullable<System.DateTime> CreadtedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> BasicSal { get; set; }
        public Nullable<int> HRA { get; set; }
        public Nullable<int> SpecialAllowance { get; set; }
        public Nullable<int> ConveyanceAllowance { get; set; }
        public Nullable<int> MealAllowance { get; set; }
        public Nullable<int> TelephoneAllowance { get; set; }
        public Nullable<int> OtherAllowance { get; set; }
        public string OtherAllowanceNotes { get; set; }
        public Nullable<int> PF { get; set; }
        public Nullable<int> IncomeTax { get; set; }
        public Nullable<int> OtherDeduction { get; set; }
        public string OtherDeductionNotes { get; set; } 
    }
    [MetadataType(typeof(tblMonthlySalCustom))]
    public partial class tblMonthySal
    {

    }
}