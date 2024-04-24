using System.ComponentModel.DataAnnotations;

namespace EventOrganizerInfrastructure.ViewModel
{
    public class RegisterForEventViewModel
    {
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Погоджуюся з правилами сервісу")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Необхідно погодитися з правилами сервісу")]
        public bool AgreeToTerms { get; set; }

       public int EventId { get; set; }
       public int RegistrationId { get; set; }
    }
}