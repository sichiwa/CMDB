using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("RoleFunctions", Schema = "CMDBMGR")]
    public class RoleFunctions
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        [Display(Name = "角色ID")]
        public int RoleID { get; set; }

        [Key]
        [Column(Order =2)]
        [Required]
        [Display(Name = "功能ID")]
        public int FunctionID { get; set; }

        [Required]
        [Display(Name = "授權")]
        public int Authority { get; set; }

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