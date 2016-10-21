using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("Tmp_CI_Profile_Relationship", Schema = "CMDBMGR")]
    public class Tmp_CI_Profile_Relationship
    {
        [Key]
        [Required]
        [Column(Order = 1)]
        [Display(Name = "範本ID")]
        public int ProfileID { get; set; }

        [Key]
        [Required]
        [Column(Order = 2)]
        [Display(Name = "目前範本ID")]
        public int oProfileID { get; set; }

        [Key]
        [Required]
        [Column(Order = 3)]
        [Display(Name = "關聯範本ID")]
        public int RelationshipProileID { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "建立者帳號")]
        public string CreateAccount { get; set; }

        [Required]
        [Display(Name = "建立時間")]
        public DateTime? CreateTime { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "覆核者帳號")]
        public string ReviewAccount { get; set; }

        [Display(Name = "覆核時間")]
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