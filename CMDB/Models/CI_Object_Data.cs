using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("CI_Object_Data", Schema = "CMDBMGR")]
    public class CI_Object_Data
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [Key]
        [Column(Order =2)]
        [Required]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Key]
        [Required]
        [Column(TypeName = "nvarchar", Order = 3)]
        [StringLength(4000)]
        [Display(Name = "屬性資料")]
        public string AttributeValue { get; set; }

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