using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vTmp_CI_Profiles_R
    {
        [Key]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "原範本ID")]
        public int oProfileID { get; set; }

        [Display(Name = "作業類型")]
        public string Type { get; set; }

        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [Display(Name = "範本說明")]
        public string Description { get; set; }

        [Display(Name = "範本圖片ID")]
        public int ImgID { get; set; }

        [Display(Name = "範本圖片")]
        public string ImgPath { get; set; }

        [Display(Name = "範本屬性")]
        public IEnumerable<vCI_Attributes> AttributesData { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }

        [Display(Name = "建立時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "範本名稱")]
        public string oProfileName { get; set; }

        [Display(Name = "範本說明")]
        public string oDescription { get; set; }

        [Display(Name = "範本圖片ID")]
        public int oImgID { get; set; }

        [Display(Name = "範本圖片")]
        public string oImgPath { get; set; }

        [Display(Name = "範本屬性")]
        public IEnumerable<vCI_Attributes> oAttributesData { get; set; }

        [Display(Name = "最後異動者")]
        public string oUpadter { get; set; }

        [Display(Name = "最後異動時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? oUpdateTime { get; set; }
    }
}