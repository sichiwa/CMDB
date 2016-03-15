using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    public class SystemInfo
    {
        [Display(Name = "版號")]
        public string ProjectVersion { get; set; }

        [Display(Name = "開發代號")]
        public string ProjectName { get; set; }

        [Display(Name = "開發軟體")]
        public string DevelopmentTools { get; set; }

        [Display(Name = "使用 dot net framework版本")]
        public string FrameworkVersion { get; set; }

        [Display(Name = "開發歷程")]
        public string DevelopmentHistory { get; set; }
    }
}