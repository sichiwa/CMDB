using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMDB.ViewModels
{
    public class vCI_Objects_List
    {
        [Key]
        public int ID { get; set; }

        public int ReviewCount { get; set; }

        public int Authority { get; set; }

        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "範本")]
        public SelectList Profile { get; set; }

        public IEnumerable<vCI_Objects> ObjectsData { get; set; }
    }
}