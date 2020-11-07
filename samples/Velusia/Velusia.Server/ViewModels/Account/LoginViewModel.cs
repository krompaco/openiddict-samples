using System.ComponentModel.DataAnnotations;

namespace Velusia.Server.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
