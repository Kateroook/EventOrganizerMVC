using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace EventOrganizerDomain.Model;

public partial class City : Entity
{
    //public int Id { get; set; }

    public int CountryId { get; set; }
    [Required(ErrorMessage ="Поле не повинно бути порожнім!")]
    //[Display(Name = "Місто")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage ="Поле не повинно бути порожнім!")]
    [Display(Name = "Країна")]
    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
