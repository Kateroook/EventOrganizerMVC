using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Registration : Entity
{
    [Display(Name ="Учасник")]
    public int UserId { get; set; }

    [Display(Name ="Подія")]
    public int EventId { get; set; }

    [Display(Name ="Зареєстровано")]
    public DateTime CreatedAt { get; set; }


    public virtual Event Event { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}