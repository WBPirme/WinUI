using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 材料库存
    {
        [Key]
        public long 材料库存ID { get; set; }

        public int? 材料ID { get; set; }

        [Required(ErrorMessage = "产品类型不能为空。")]
        [MaxLength(20, ErrorMessage = "产品类型不能超过 20 个字符。")]
        public string 材料类型 { get; set; }

        [Required(ErrorMessage = "数量不能为空。")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "数量必须为非负数。")]
        public decimal 数量 { get; set; }

        [Required(ErrorMessage = "计量单位不能为空。")]
        [MaxLength(10, ErrorMessage = "计量单位不能超过 10 个字符。")]
        public string 计量单位 { get; set; }
        [Required]
        public DateTime 最后更新时间 { get; set; } = DateTime.Now;

        [MaxLength(50, ErrorMessage = "备注不能超过 50 个字符。")]
        public string? 备注 { get; set; }

        // 外键关系
        [ForeignKey("材料ID")]
        public 材料 材料 { get; set; }

    }
}
