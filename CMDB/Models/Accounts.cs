using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Accounts", Schema = "CMDBMGR")]
    public class Accounts
    {
        [Key]
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "帳號")]
        public string Account { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(10)]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(300)]
        [Display(Name = "密碼")]
        public string Pwd { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "角色代碼")]
        public int RoleID { get; set; }

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

        [Required]
        [Display(Name = "是否啟用")]
        public bool isEnable { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(300)]
        [Display(Name = "資料HASH值")]
        public string HashValue { get; set; }
    }
}