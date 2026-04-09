using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OGAS.Models
{
    public class 出口订单
    {
        [Key]
        public long 订单ID { get; set; }

        [Required]
        public DateTime 订单日期 { get; set; } = DateTime.Now;

        [Required]
        public int 产品ID { get; set; }

        [Required(ErrorMessage = "出口数量不能为空。")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "出口数量必须大于 0。")]
        public decimal 出口数量 { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "收益必须为非负数。")]
        public decimal 收益 { get; set; }

        // 外键导航属性
        public 产品 产品 { get; set; }  // 确保这个导航属性已经正确定义
    }

}
