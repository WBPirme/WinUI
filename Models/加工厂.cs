using OGAS.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 加工厂
    {
        [Key]
        public int 加工厂ID { get; set; }

        [Required(ErrorMessage = "加工厂名称不能为空。")]
        [MaxLength(20, ErrorMessage = "加工厂名称不能超过 20 个字符。")]
        public string 名称 { get; set; }

        public int? 生产设备ID { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "设备数量必须为非负数。")]
        public int? 设备数量 { get; set; }

        [MaxLength(50, ErrorMessage = "位置不能超过 50 个字符。")]
        public string? 位置 { get; set; }

        [Required(ErrorMessage = "加工效率不能为空。")]
        [Column(TypeName = "decimal(3,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "加工效率必须为非负数。")]
        public decimal 加工效率 { get; set; }

        [MaxLength(50, ErrorMessage = "联系人信息不能超过 50 个字符。")]
        public string? 联系人信息 { get; set; }
        public string? 状态 { get; set; }

        // 导航属性
        [ForeignKey("生产设备ID")]
        public 生产设备 生产设备 { get; set; }
        public ICollection<生产计划> 生产计划s { get; set; }
        public ICollection<生产记录> 生产记录s { get; set; }
    }

}
