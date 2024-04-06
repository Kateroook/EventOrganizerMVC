using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Country : Entity
{
    [Required(ErrorMessage ="Поле не повинно бути порожнім!")]
    [Display(Name="Країна")]
    public string Name { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
