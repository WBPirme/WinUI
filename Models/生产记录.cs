using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 生产记录
    {
        [Key]
        public long 生产记录ID { get; set; }

        [MaxLength(10, ErrorMessage = "记录类型不能超过 10 个字符。")]
        public string 记录类型 { get; set; }

        [Required]
        public long 计划ID { get; set; }

        [MaxLength(20, ErrorMessage = "材料来源不能超过 20 个字符。")]
        public string? 材料来源 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "材料数量必须为非负数。")]
        public decimal? 材料数量 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "材料成本必须为非负数。")]
        public decimal? 材料成本 { get; set; }

        [Required]
        public int 加工厂ID { get; set; }

        [Required]
        public long 工艺ID { get; set; }

        [Required(ErrorMessage = "实际开始日期不能为空。")]
        public DateTime 实际开始日期 { get; set; }

        public DateTime? 实际结束日期 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "生产数量必须为非负数。")]
        public decimal? 生产数量 { get; set; }

        [Required(ErrorMessage = "生产时间不能为空。")]
        public decimal? 生产时间 { get; set; }

        [MaxLength(20, ErrorMessage = "质量指标不能超过 20 个字符。")]
        public string? 质量指标 { get; set; }

        [MaxLength(50, ErrorMessage = "备注不能超过 50 个字符。")]
        public string? 备注 { get; set; }

        // 外键关系
        [ForeignKey("工艺ID")]
        public 生产工艺 生产工艺 { get; set; }

        [ForeignKey("加工厂ID")]
        public 加工厂 加工厂 { get; set; }

        [ForeignKey("计划ID")]
        public 生产计划 生产计划 { get; set; }

        [Required]
        public int 生产设备ID { get; set; }

        [ForeignKey("生产设备ID")]
        public 生产设备 生产设备 { get; set; }

    }
}
