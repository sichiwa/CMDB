using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("CI_Proflie_Attributes", Schema = "CMDBMGR")]
    public class CI_Proflie_Attributes
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Key]
        [Column(Order =2)]
        [Required]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Required]
        [Display(Name = "屬性排序")]
        public int AttributeOrder { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "建立者帳號")]
        public string CreateAccount { get; set; }

        [Required]
        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "最後異動者帳號")]
        public string UpdateAccount { get; set; }

        [Required]
        [Display(Name = "最後異動時間")]
        public DateTime? UpdateTime { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(1000)]
        [Display(Name = "資料HASH值")]
        public string HashValue { get; set; }
    }
}