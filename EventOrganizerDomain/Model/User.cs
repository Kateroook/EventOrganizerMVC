using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventOrganizerDomain.Model;

public partial class User : Entity
{

    public int RoleId { get; set; }

    public string? FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; } = string.Empty ;
    public string FullName => FirstName + " " + LastName;
    public string? OrganizationName { get; set; } = string.Empty;

    public string OrganizationOrFullName => string.IsNullOrEmpty(OrganizationName) ? FullName : string.IsNullOrEmpty(LastName) ? OrganizationName : $"{FullName} ({OrganizationName})";
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Info { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; }

    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<Event> Events { get; set; } 
}
