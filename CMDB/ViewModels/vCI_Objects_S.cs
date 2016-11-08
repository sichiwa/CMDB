using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CMDB.Models;
using System.Web.Mvc;

namespace CMDB.ViewModels
{
    public class vCI_Objects_S
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [Display(Name = "物件名稱")]
        public string ObjectName { get; set; }

        public IEnumerable<vCI_Objects> ObjectsData { get; set; }

    }
}