using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("CI_Proflies", Schema = "CMDBMGR")]
    public class CI_Proflies
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "流水號")]
        public int SN { get; set; }

        [Required]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

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