using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Tmp_CI_Proflies", Schema = "CMDBMGR")]
    public class Tmp_CI_Proflies
    {
        [Key]
        [Required]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Display(Name = "目前範本ID")]
        public int oProfileID { get; set; }

        [Required]
        [Display(Name = "範本圖片ID")]
        public int ImgID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Display(Name = "範本名稱")]
        public string ProfileName { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        [Display(Name = "範本說明")]
        public string Description { get; set; }

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