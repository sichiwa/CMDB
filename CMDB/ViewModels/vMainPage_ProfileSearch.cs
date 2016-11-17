using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vMainPage_ProfileSearch
    {
        [Key]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [Display(Name = "物件個數")]
        public int UsedObjectCount { get; set; }

        [Display(Name = "範本ID")]
        public int ImgID { get; set; }

        [Display(Name = "範本圖片")]
        public string ImgPath { get; set; }
    }
}