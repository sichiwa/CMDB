using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMDB.ViewModels
{
    public class vMainPage
    {
        [Key]
        [Display(Name = "流水號")]
        public int SN { get; set; }

        [Display(Name = "一列呈現幾個Profile")]
        public int NumberOfProfilePerRow { get; set; }

        [Display(Name = "Profile個數")]
        public int NumberOfProfile { get; set; }

        public IEnumerable<vMainPage_ProfileSearch> ProfileSearchList { get; set; }
    }
}