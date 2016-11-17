using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Tmp_Accounts", Schema = "CMDBMGR")]
    public class Tmp_Accounts
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
        [StringLength(200)]
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
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? CreateTime { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "覆核者帳號")]
        public string ReviewAccount { get; set; }

        [Required]
        [Display(Name = "覆核時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? ReviewTime { get; set; }

        [Required]
        [Display(Name = "是否啟用")]
        public bool isEnable { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(10)]
        [Display(Name = "類型")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "是否完成覆核")]
        public bool isClose { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(1000)]
        [Display(Name = "資料HASH值")]
        public string HashValue { get; set; }
    }
}