using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMDB.ViewModels
{
    public class vCI_Object_Relationship_CU
    {
        [Key]
        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "關聯物件ID")]
        //[Required(ErrorMessage = "請至少選擇一項")]
        public int[] RelationshipObjectID { get; set; }

        [Display(Name = "關聯範本ID")]
        public int RelationshipProfileID { get; set; }

        [StringLength(10)]
        [Display(Name = "建立者帳號")]
        public string CreateAccount { get; set; }

        [Display(Name = "建立時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? CreateTime { get; set; }

        [StringLength(10)]
        [Display(Name = "最後異動者帳號")]
        public string UpdateAccount { get; set; }

        [Display(Name = "最後異動時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? UpdateTime { get; set; }

    }
}