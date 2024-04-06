using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Tag : Entity
{
    [Display(Name ="Назва")]
    public string Title { get; set; } = null!;

    [Display(Name ="Опис")]
    public string? Description { get; set; }


    public ICollection<Event> Events { get; set; } = new List<Event>();
}
