using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CMDB.Models;

namespace CMDB.ViewModels
{
    public class vCI_Proflies_CU
    {
        [Key]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [StringLength(50)]
        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [StringLength(200)]
        [Display(Name = "範本說明")]
        public string Description { get; set; }

        [Display(Name = "範本圖片ID")]
        public int ImgID { get; set; }

        [Display(Name = "範本圖片")]
        public string ImgPath { get; set; }

        [Display(Name = "範本屬性")]
        public IEnumerable<vCI_Attributes> AttributesData { get; set; }
    }
}