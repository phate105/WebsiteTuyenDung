using System.ComponentModel.DataAnnotations;

namespace WebsiteTuyenDung.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, ErrorMessage = "Mật khẩu tối đa 100 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
        [StringLength(50)]
        public string VaiTro { get; set; }
    }

    public sealed class DemoAccountInfo
    {
        public DemoAccountInfo(string key, string roleLabel, string email, string password, string description)
        {
            Key = key;
            RoleLabel = roleLabel;
            Email = email;
            Password = password;
            Description = description;
        }

        public string Key { get; }

        public string RoleLabel { get; }

        public string Email { get; }

        public string Password { get; }

        public string Description { get; }
    }
}
