using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Tmp_CI_Attributes", Schema = "CMDBMGR")]
    public class Tmp_CI_Attributes
    {
        [Key]
        [Required]
        [Display(Name = "屬性ID")]
        public int AttributeID { get; set; }

        [Display(Name = "目前屬性ID")]
        public int oAttributeID { get; set; }

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