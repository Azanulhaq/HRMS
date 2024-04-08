using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace PureHRMS
{
    public class tblUserCustom
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Designation is required")]
        public int DesignationId { get; set; }
        public string ImagePath { get; set; }
        public string HighestQualification { get; set; }
        public string SecondHighestQualification { get; set; }
        [AllowHtml]
        public string OtherDetails { get; set; }
        public string UserPassword { get; set; }
        public bool Active { get; set; }
        public string UserType { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime Dob { get; set; }
        public string EmpId { get; set; }
        public Nullable<bool> IsWebMaster { get; set; }
    }
    [MetadataType(typeof(tblUserCustom))]
    public partial class tblUserMaster
    {

    }
}