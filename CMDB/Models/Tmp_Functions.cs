using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Tmp_Functions", Schema = "CMDBMGR")]
    public class Tmp_Functions
    {
        [Key]
        [Required]
        public int FunctionID { get; set; }

        [Required]
        [Display(Name = "功能編號")]
        public int FId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "功能名稱")]
        public string FunctionName { get; set; }

        [Display(Name = "父層功能編號")]
        public int ParentID { get; set; }

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