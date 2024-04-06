using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class PlaceType : Entity
{
    [Display(Name="Тип місця")]
    public string Name { get; set; } = null!;

    [Display(Name="Опис")]
    public string? Description { get; set; }

    public virtual ICollection<Place> Places { get; set; } = new List<Place>();
}
