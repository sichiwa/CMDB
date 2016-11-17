using System;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vTmp_CI_Profile_Relationship
    {
        [Key]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [Display(Name = "關係範本名稱")]
        public string RelationProfileName { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }

        [Display(Name = "建立時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "作業類型")]
        public string Type { get; set; }

    }
}