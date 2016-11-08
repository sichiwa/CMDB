using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vCI_Object_Relationship
    {
        [Key]
        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [Display(Name = "物件名稱")]
        public string ObjectName { get; set; }

        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [Display(Name = "關聯物件ID")]
        public int RelationshipObjectID { get; set; }

        [Display(Name = "關聯物件名稱")]
        public string RelationshipObjectName { get; set; }

        [Display(Name = "關聯物件名稱")]
        public List<string> RelationshipObjectNames { get; set; }

        [Display(Name = "關聯範本ID")]
        public int RelationshipProfileID { get; set; }

        [Display(Name = "關聯範本名稱")]
        public string RelationshipProfileName { get; set; }

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