using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vCI_Profile_Relationship
    {
        [Key]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [Display(Name = "關聯範本ID")]
        public int RelationshipProfileID { get; set; }

        [Display(Name = "關聯範本名稱")]
        public string RelationshipProfileName { get; set; }

        [Display(Name = "關聯範本名稱")]
        public List<string> RelationshipProfileNames { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }

        [Display(Name = "建立時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "最後異動者")]
        public string Upadter { get; set; }

        [Display(Name = "最後異動時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? UpdateTime { get; set; }

        public string EditAccount { get; set; }
    }
}