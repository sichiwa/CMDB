using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Functions", Schema = "CMDBMGR")]
    public class Functions
    {
        [Key]
        [Required]
        public int SN { get; set; }

        [Required]
        [Display(Name = "功能編號")]
        public int FunctionID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(20)]
        [Display(Name = "功能名稱")]
        public string FunctionName { get; set; }

        [Display(Name = "父層功能編號")]
        public int? ParentID { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        [Display(Name = "對應controller名稱")]
        public string Controller { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        [Display(Name = "對應action名稱")]
        public string Action { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        [Display(Name = "對應Url名稱")]
        public string Url { get; set; }

        [Display(Name = "顯示順序")]
        public int ShowOrder { get; set; }

        [Required]
        [Display(Name = "是否啟用")]
        public bool IsEnable { get; set; }

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
        [StringLength(6)]
        [Display(Name = "最後異動者帳號")]
        public string UpdateAccount { get; set; }

        [Required]
        [Display(Name = "最後異動時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime? UpdateTime { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(1000)]
        [Display(Name = "資料HASH值")]
        public string HashValue { get; set; }
    }
}