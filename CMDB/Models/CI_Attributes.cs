using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("CI_Attributes", Schema = "CMDBMGR")]
    public class CI_Attributes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "流水號")]
        public int SN { get; set; }

        [Required]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Display(Name = "屬性名稱")]
        public string AttributeName { get; set; }

        [Display(Name = "屬性類型")]
        public int AttributeTypeID { get; set; }

        [Display(Name = "允許多值")]
        public bool AllowMutiValue { get; set; }

        [Display(Name = "允許搜尋")]
        public bool AllowSearch { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(1000)]
        [Display(Name = "屬性下拉選單值")]
        public string DropDownValues { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        [Display(Name = "屬性說明")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "建立者帳號")]
        public string CreateAccount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "最後異動者帳號")]
        public string UpdateAccount { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        [Display(Name = "最後異動時間")]
        public DateTime? UpdateTime { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(1000)]
        [Display(Name = "資料HASH值")]
        public string HashValue { get; set; }
    }
}