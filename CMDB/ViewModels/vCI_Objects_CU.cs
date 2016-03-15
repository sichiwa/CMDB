using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CMDB.Models;
using System.Web.Mvc;

namespace CMDB.ViewModels
{
    public class vCI_Objects_CU
    {
        [Key]
        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [StringLength(50)]
        [Display(Name = "物件名稱")]
        public string ObjectName { get; set; }

        [StringLength(200)]
        [Display(Name = "物件說明")]
        public string Description { get; set; }

        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本")]
        public SelectList Profile { get; set; }

        [Display(Name = "物件屬性")]
        public IEnumerable<vCI_Attributes> AttributesData { get; set; }
    }
}