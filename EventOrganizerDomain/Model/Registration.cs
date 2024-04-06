using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Registration : Entity
{
    //public int Id { get; set; }

    public int UserId { get; set; }

    public int EventId { get; set; }

    [Display(Name ="Зареєствровано о")]
    public DateTime CreatedAt { get; set; }

    [Display(Name ="Оновлнено о")]
    public DateTime LastUpdatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
