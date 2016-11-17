using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vCI_Objects
    {
        [Key]
        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [Display(Name = "物件名稱")]
        public string ObjectName { get; set; }

        [Display(Name = "套用範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "套用範本")]
        public string ProfileName { get; set; }

        [Display(Name = "物件說明")]
        public string Description { get; set; }

        [Display(Name = "物件屬性值")]
        public IEnumerable<vCI_Attributes> AttributesData { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "最後異動者")]
        public string Upadter { get; set; }

        [Display(Name = "最後異動時間")]
        public DateTime? UpdateTime { get; set; }

        public string EditAccount { get; set; }
    }
}