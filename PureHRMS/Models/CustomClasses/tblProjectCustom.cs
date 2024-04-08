using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
    public class tblProjectCustom
    {
        [Key]
        public int ProjectId { get; set; }
        [Required(ErrorMessage ="Start Date is required")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [Required(ErrorMessage = "Estimated End Date is required")]
        public Nullable<System.DateTime> EstEndDate { get; set; }
        public Nullable<System.DateTime> ActEndDate { get; set; }
        public string ProjectStatus { get; set; }     
        [Required(ErrorMessage = "Project title is required")]
        public string ProjectTitle { get; set; } 
        [AllowHtml]     
        public string ProjectNotes { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> Createdby { get; set; }
    }
    [MetadataType(typeof(tblProjectCustom))]
    partial class tblProjectMaster
    {
    }
}