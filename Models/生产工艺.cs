using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 生产工艺
    {
        [Key]
        public long 工艺ID { get; set; }

        [Required]
        public int 产品ID { get; set; }

        public int? 材料ID { get; set; }

        [Required]
        public int 生产设备ID { get; set; }

        [Required(ErrorMessage = "工艺名称不能为空。")]
        [MaxLength(20, ErrorMessage = "工艺名称不能超过 20 个字符。")]
        public string 工艺名称 { get; set; }

        public int 标准加工时间 { get; set; }

        [MaxLength(10, ErrorMessage = "质量标准不能超过 10 个字符。")]
        public string? 质量标准 { get; set; }

        [MaxLength(50, ErrorMessage = "安全指引不能超过 50 个字符。")]
        public string? 安全指引 { get; set; }

        [MaxLength(50, ErrorMessage = "备注不能超过 50 个字符。")]
        public string? 备注 { get; set; }

        // 外键关系
        [ForeignKey("产品ID")]
        public 产品 产品 { get; set; }

        [ForeignKey("材料ID")]
        public 材料 材料 { get; set; }

        [ForeignKey("生产设备ID")]
        public 生产设备 生产设备 { get; set; }

        // 导航属性
        public ICollection<生产计划> 生产计划s { get; set; }
        public ICollection<生产记录> 生产记录s { get; set; }

    }
}
