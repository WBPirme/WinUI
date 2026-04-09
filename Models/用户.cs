using System.ComponentModel.DataAnnotations;

namespace OGAS.Models
{
    public class 用户
    {
        [Key]
        public string 编号 { get; set; } // Username

        [Required]
        public string 密码 { get; set; } // Password

        // 其他属性（如姓名、角色等）可以根据需要添加
    }
}