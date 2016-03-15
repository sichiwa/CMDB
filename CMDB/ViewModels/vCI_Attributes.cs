using System;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vCI_Attributes
    {
        [Key]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Display(Name = "屬性名稱")]
        public string AttributeName { get; set; }

        [Display(Name = "屬性值")]
        public string AttributeValue { get; set; }

        [Display(Name = "屬性類型ID")]
        public int AttributeTypeID { get; set; }

        [Display(Name = "屬性類型")]
        public string AttributeTypeName { get; set; }

        [Display(Name = "屬性說明")]
        public string Description { get; set; }

        [Display(Name = "選單值")]
        public string DropDownValues { get; set; }

        [Display(Name = "屬性順序")]
        public int AttributeOrder { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }
        
        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }
       
        [Display(Name = "最後異動者")]
        public string Upadter{ get; set; }
      
        [Display(Name = "最後異動時間")]
        public DateTime? UpdateTime { get; set; }

        public string EditAccount { get; set; }
    }
}