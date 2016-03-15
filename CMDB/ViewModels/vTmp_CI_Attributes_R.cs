using System;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vTmp_CI_Attributes_R
    {
        [Key]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Display(Name = "原屬性ID")]
        public int oAttributeID { get; set; }

        [Display(Name = "作業類型")]
        public string Type { get; set; }

        [Display(Name = "屬性名稱")]
        public string AttributeName { get; set; }

        [Display(Name = "屬性說明")]
        public string Description { get; set; }

        [Display(Name = "屬性類型")]
        public string AttributeTypeName { get; set; }

        [Display(Name = "選單值")]
        public string DropDownValues { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "屬性名稱")]
        public string oAttributeName { get; set; }

        [Display(Name = "屬性說明")]
        public string oDescription { get; set; }

        [Display(Name = "屬性類型")]
        public string oAttributeTypeName { get; set; }

        [Display(Name = "選單值")]
        public string oDropDownValues { get; set; }

        [Display(Name = "最後異動者")]
        public string oUpadter { get; set; }

        [Display(Name = "最後異動時間")]
        public DateTime? oUpdateTime { get; set; }
    }
}