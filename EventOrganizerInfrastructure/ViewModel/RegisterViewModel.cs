using System.ComponentModel.DataAnnotations;

namespace EventOrganizerInfrastructure.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [Display(Name="Пароль")]
        public string Password { get; set; }

        [Display(Name ="Номер телефону")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage ="Паролі не співпадають")]
        [Display(Name ="Підтвердження паролю")]
        [DataType(DataType.Password)]
        public string PasswordConfirmed { get; set; } = string.Empty;
        //public string PhoneNumberConfirmed { get; set;} = string.Empty; 
    }
}