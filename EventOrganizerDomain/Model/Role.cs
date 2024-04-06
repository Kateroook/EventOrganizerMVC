using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Role : IdentityRole<int>
{

    [Display(Name="Роль")]
    public string Name { get; set; } = null!;
    
    [Display(Name ="Опис")]
    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
