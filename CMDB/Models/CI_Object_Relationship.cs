using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("CI_Object_Relationship", Schema = "CMDBMGR")]
    public class CI_Object_Relationship
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "流水號")]
        public int SN { get; set; }

        [Required]
        [Display(Name = "物件ID")]
        public int ObjectID { get; set; }

        [Required]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Required]
        [Display(Name = "關聯物件ID")]
        public int RelationshipObjectID { get; set; }

        [Required]
        [Display(Name = "關聯範本ID")]
        public int RelationshipProfileID { get; set; }

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