using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PureHRMS
{
   
    public class tblDepartmentCustom
    {

        [Key]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "Department name is required")]
        public string DepartmentName { get; set; }

    }
    [MetadataType(typeof(tblDepartmentCustom))]
    public partial class tblDepartmentMaster
    {
    }
  
}