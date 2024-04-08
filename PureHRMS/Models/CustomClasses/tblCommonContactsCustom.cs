using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PureHRMS
{
    public class tblCommonContactsCustom
    {
        [Key]
        public int ContactId { get; set; }
        public string ContactText { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
    [MetadataType(typeof(tblCommonContactsCustom))]
    public partial class tblCommonContact
    {

    }
}