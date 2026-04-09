using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 生产设备
    {
        [Key]
        public int 生产设备ID { get; set; }

        [Required(ErrorMessage = "设备名称不能为空。")]
        [MaxLength(50, ErrorMessage = "设备名称不能超过 50 个字符。")]
        public string 名称 { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "描述不能超过 50 个字符。")]
        public string? 描述 { get; set; }

        [Required(ErrorMessage = "容量不能为空。")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "容量必须为非负数。")]
        public decimal 容量 { get; set; }

        [Required(ErrorMessage = "效率不能为空。")]
        [Column(TypeName = "decimal(3,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "效率必须为非负数。")]
        public decimal 效率 { get; set; }

        public ICollection< 加工厂> 加工厂s { get; set; }
        public ICollection<生产记录> 生产记录s { get; set; }
        public ICollection<生产工艺> 生产工艺s { get; set; }
    }
}
