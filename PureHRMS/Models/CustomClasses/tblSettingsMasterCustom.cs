using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblSettingsMasterCustom
    {
        public int Cid { get; set; }
        [Required(ErrorMessage ="Company name is required")]
        public string CompanyNameSal { get; set; }
        [Required(ErrorMessage = "Company short name is required")]
        public string CompanyShortName { get; set; }
        public Nullable<System.TimeSpan> InTime { get; set; }
        public Nullable<System.TimeSpan> OutTime { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyInstructionForSal { get; set; }
        public string AddressContactDetails { get; set; }
    }
    [MetadataType(typeof(tblSettingsMasterCustom))]
    public partial class tblSettingsMaster
    {

    }
}