using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMDB.ViewModels
{
    public class vCI_Attributes_CU
    {
        [Key]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [StringLength(50)]
        [Display(Name = "屬性名稱")]
        public string AttributeName { get; set; }

        [StringLength(200)]
        [Display(Name = "屬性說明")]
        public string Description { get; set; }

        [Display(Name = "屬性類型ID")]
        public int AttributeTypeID { get; set; }

        [Display(Name = "屬性類型")]
        public SelectList AttributeType { get; set; }

        [StringLength(1000)]
        [Display(Name = "選單值")]
        public string DropDownValues { get; set; }
    }
}