using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Role : IdentityRole<int>
{

    public Role() : base()
    {

    }
    public Role(string roleName) : this()
    {
        Name = roleName;
    }

    //[Display(Name="Роль")]
    //public string Name { get; set; }
    
    [Display(Name ="Опис")]
    public string? Description { get; set; } = string.Empty;
    public virtual ICollection<IdentityUserRole<int>> Users { get; set; }
    //public virtual ICollection<User> Users { get; set; } = new List<User>();
}
