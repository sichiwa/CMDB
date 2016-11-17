using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CMDB.ViewModels
{
    public class vSearchKeywords
    {
        [Key]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Display(Name = "關鍵字")]
        public string Keyword { get; set; }
        
    }
}