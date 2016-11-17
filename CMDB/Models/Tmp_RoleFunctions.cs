using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Tmp_RoleFunctions", Schema = "CMDBMGR")]
    public class Tmp_RoleFunctions
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        [Display(Name = "角色ID")]
        public int RoleID { get; set; }

        [Key]
        [Column(Order = 2)]
        [Required]
        [Display(Name = "功能ID")]
        public int FId { get; set; }

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
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? CreateTime { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "覆核者帳號")]
        public string ReviewAccount { get; set; }

        [Display(Name = "覆核時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? ReviewTime { get; set; }

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