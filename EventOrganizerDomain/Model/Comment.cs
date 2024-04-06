using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Comment : Entity
{
    [Display(Name ="Користувач")]
    public int UserId { get; set; }

    [Display(Name ="Подія")]
    public int EventId { get; set; }

    [Display(Name ="Коментар")]
    public string Text { get; set; } = null!;

    [Display(Name ="Створено")]
    public DateTime CreatedAt { get; set; }

    [Display(Name ="Оновлено")]
    public DateTime LastUpdatedAt { get; set; }


    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
