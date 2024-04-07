using System.ComponentModel.DataAnnotations;

namespace EventOrganizerInfrastructure.ViewModel
{
    public class LoginViewModel
    {
        private string returnUrl;

        [Required]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name ="Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Запам`ятати?")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get => returnUrl; set => returnUrl = value; }
    }
}
