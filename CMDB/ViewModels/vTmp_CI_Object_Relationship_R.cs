using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vTmp_CI_Object_Relationship_R
    {
        [Key]
        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "原物件ID")]
        public int oObjectID { get; set; }

        [Display(Name = "原範本ID")]
        public int oProfileID { get; set; }

        [Display(Name = "物件名稱")]
        public string ObjectName { get; set; }

        [Display(Name = "作業類型")]
        public string Type { get; set; }

        [Display(Name = "範本關係")]
        public IEnumerable<vCI_Object_Relationship> ObjectRelationshipData { get; set; }

        [Display(Name = "建立者")]
        public string Creator { get; set; }

        [Display(Name = "建立時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "物件名稱")]
        public string oObjectName { get; set; }

        [Display(Name = "範本關係")]
        public IEnumerable<vCI_Object_Relationship> oObjectRelationshipData { get; set; }

        [Display(Name = "最後異動者")]
        public string oUpadter { get; set; }

        [Display(Name = "最後異動時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? oUpdateTime { get; set; }

    }
}