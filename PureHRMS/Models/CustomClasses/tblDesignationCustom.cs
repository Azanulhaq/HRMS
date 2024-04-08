using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblDesignationCustom
    {
        [Key]
        public int DesignationId { get; set; }
        [Required(ErrorMessage = "Designation name is required")]
        public string DesignationName { get; set; }
        [Required(ErrorMessage = "Department is required")]
        public int RefDepartmentId { get; set; }
    }

    [MetadataType(typeof(tblDesignationCustom))]
    public partial class tblDesignationMaster
    {
    }
}