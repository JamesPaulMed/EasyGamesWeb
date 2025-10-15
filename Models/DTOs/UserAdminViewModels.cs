using System.ComponentModel.DataAnnotations;

namespace EasyGamesWeb.Models.DTOs
{
    public class UserRowVM
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class UserCreateVM
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Role { get; set; } = "User";
    }

    public class UserEditVM
    {
        [Required]
        public string Id { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Role { get; set; } = "User";
    }

    public class ResetPasswordVM
    {
        [Required]
        public string Id { get; set; } = "";

        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [MinLength(6)]
        public string NewPassword { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = "";
    }
}
