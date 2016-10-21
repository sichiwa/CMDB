using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMDB.ViewModels
{
    public class vCI_Profile_Relationship_CU
    {
        [Key]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本選單")]
        public SelectList ProfileList { get; set; }

        [Display(Name = "關聯範本ID")]
        [Required(ErrorMessage = "請至少選擇一項")]
        public int[] RelationshipProileID { get; set; }

        [Display(Name = "範本選單")]
        public SelectList RelationshipProfileList { get; set; }

        [StringLength(10)]
        [Display(Name = "建立者帳號")]
        public string CreateAccount { get; set; }

        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [StringLength(10)]
        [Display(Name = "最後異動者帳號")]
        public string UpdateAccount { get; set; }

        [Display(Name = "最後異動時間")]
        public DateTime? UpdateTime { get; set; }

    }
}