using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
    public class tblPayrollCustom
    {
        [Key]
        public int SId { get; set; }
        [Required]
        public Nullable<int> refEID { get; set; }
        [Required (ErrorMessage ="Basic salary is required")]
        public Nullable<int> BasicSalary { get; set; }
        [Required(ErrorMessage = "HRA is required , if don't want please put 0 in this field ")]
        public Nullable<int> HRA { get; set; }
        [Required(ErrorMessage = "Special Allowance is required , if don't want please put 0 in this field")]
        public Nullable<int> SpecialAllowance { get; set; }
        [Required(ErrorMessage = "Conveyance Allowance is required , if don't want please put 0 in this field")]
        public Nullable<int> ConveyanceAllowance { get; set; }
        [Required(ErrorMessage = "Meal Allowance is required , if don't want please put 0 in this field")]
        public Nullable<int> MealAllowance { get; set; }
        [Required(ErrorMessage = "Telephone Allowance is required , if don't want please put 0 in this field")]
        public Nullable<int> TelephoneAllowance { get; set; }
        [Required(ErrorMessage = "Other Allowance is required , if don't want please put 0 in this field")]
        public Nullable<int> OtherAllowance { get; set; }
        public string OtherAllowanceNotes { get; set; }
        [Required(ErrorMessage = "PF is required , if don't want please put 0 in this field")]
        public Nullable<int> PF { get; set; }
        [Required(ErrorMessage = "Income Tax is required , if don't want please put 0 in this field")]
        public Nullable<int> IncomeTax { get; set; }
        [Required(ErrorMessage = "Other Deduction is required , if don't want please put 0 in this field")]
        public Nullable<int> OtherDeduction { get; set; }
        public string OtherDeductionNotes { get; set; }
    }
    [MetadataType(typeof(tblPayrollCustom))]
    public partial class tblSalaryMaster
    {

    }
}