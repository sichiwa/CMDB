using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMDB.Models
{
    [Table("SystemLogs", Schema = "CMDBMGR")]
    public class SystemLogs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name = "唯一流水號")]
        public int SN { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(10)]
        [Display(Name = "執行帳號")]
        public string Account { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar")]
        [Display(Name = "處理模組名稱")]
        public string Controller { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar")]
        [Display(Name = "執行作業名稱")]
        public string Action { get; set; }

        [Required]
        [Display(Name = "起始時間")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "結束時間")]
        public DateTime EndTime { get; set; }

        [Display(Name = "處理總筆數")]
        public int TotalCount { get; set; }

        [Display(Name = "處理成功筆數")]
        public int SuccessCount { get; set; }

        [Display(Name = "處理失敗筆數")]
        public int FailCount { get; set; }

        [Required]
        [Display(Name = "處理結果")]
        public Boolean Result { get; set; }

        [StringLength(1000)]
        [Column(TypeName = "nvarchar")]
        [Display(Name = "作業訊息")]
        public string Msg { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(1000)]
        [Display(Name = "資料HASH值")]
        public string HashValue { get; set; }
    }
}