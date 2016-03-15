using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vCI_Proflies
    {
        [Key]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [Display(Name = "範本說明")]
        public string Description { get; set; }

        [Display(Name = "範本圖片ID")]
        public int ImgID { get; set; }

        [Display(Name = "範本圖片")]
        public string ImgPath { get; set; }

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