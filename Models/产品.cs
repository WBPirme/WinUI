using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 产品
    {
        [Key]
        public int 产品ID { get; set; }

        [Required(ErrorMessage = "产品类别不能为空。")]
        [MaxLength(10, ErrorMessage = "产品类别不能超过 10 个字符。")]
        public string 产品类别 { get; set; }

        [Required(ErrorMessage = "名称不能为空。")]
        [MaxLength(50, ErrorMessage = "名称不能超过 50 个字符。")]
        public string 名称 { get; set; }

        [MaxLength(200, ErrorMessage = "描述不能超过 200 个字符。")]
        public string? 描述 { get; set; }

        [Required(ErrorMessage = "计量单位不能为空。")]
        [MaxLength(10, ErrorMessage = "计量单位不能超过 10 个字符。")]
        public string 计量单位 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "每单位价格必须为非负数。")]
        public decimal 每单位价格 { get; set; }

        // 导航属性
        public ICollection<生产工艺> 生产工艺s { get; set; }
        public ICollection<出口订单> 出口订单s { get; set; }  // 确保这个导航属性已经正确定义
        public ICollection<产品库存> 产品库存s { get; set; }
    }

}
