using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 生产计划 
    {
        [Key]
        public long 计划ID { get; set; }

        [Required(ErrorMessage = "计划类型不能为空。")]
        [MaxLength(10, ErrorMessage = "计划类型不能超过 10 个字符。")]
        public string 计划类型 { get; set; }

        public int 加工厂ID { get; set; }

        [Required]
        public long 工艺ID { get; set; }

        [Required(ErrorMessage = "计划开始日期不能为空。")]
        public DateTime 计划开始日期 { get; set; }

        [Required(ErrorMessage = "计划数量不能为空。")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "计划数量必须为非负数。")]
        public decimal 计划数量 { get; set; }

        [MaxLength(10, ErrorMessage = "状态不能超过 10 个字符。")]
        public string 状态 { get; set; }

        [MaxLength(50, ErrorMessage = "备注不能超过 50 个字符。")]
        public string? 备注 { get; set; }

        // 外键关系
        [ForeignKey("加工厂ID")]
        public 加工厂 加工厂 { get; set; }

        [ForeignKey("工艺ID")]
        public 生产工艺 生产工艺 { get; set; }
        public ICollection<生产记录> 生产记录s { get; set; }
    }
}
