using System.ComponentModel.DataAnnotations;
 
namespace csharp_lend_pc.Models
{
    // AccountController の Login 用
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "ユーザー名")]
        [StringLength(100, ErrorMessage =
            "{0}は{2}から{1}文字の範囲で設定してください。",
            MinimumLength = 6)]
        public string UserName { get; set; }
 
        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(100, ErrorMessage =
            "{0}は{2}から{1}文字の範囲で設定してください。",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        [StringLength(100, ErrorMessage =
            "{0}は{2}から{1}文字の範囲で設定してください。",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード(確認)")]
        public string ConfirmPassword { get; set; }
    }
}