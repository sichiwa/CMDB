using System;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vTmp_CI_Attributes
    {
        [Key]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Display(Name = "屬性名稱")]
        public string AttributeName { get; set; }

        [Display(Name = "屬性說明")]
        public string Description { get; set; }

        [Display(Name = "屬性類型")]
        public string AttributeTypeName { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "作業類型")]
        public string Type { get; set; }
    }
}