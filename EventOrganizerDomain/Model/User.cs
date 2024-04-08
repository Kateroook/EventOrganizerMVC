using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace EventOrganizerDomain.Model;

public partial class User : IdentityUser<int>
{
    //[Display(Name ="Роль")]
    //public int RoleId { get; set; }

    [Display(Name ="Ім'я")]
    public string? FirstName { get; set; } = string.Empty;

    [Display(Name ="Прізвище")]
    public string? LastName { get; set; } = string.Empty ;

    [Display(Name ="Повне ім'я")]
    public string FullName => FirstName + " " + LastName;

    [Display(Name ="Організація")]
    public string? OrganizationName { get; set; } = string.Empty;

    public string OrganizationOrFullName => string.IsNullOrEmpty(OrganizationName) ? FullName : string.IsNullOrEmpty(LastName) ? OrganizationName : $"{FullName} ({OrganizationName})";
    
    [Display(Name = "Пароль")]
    public string? Password { get; set; }

    [Display(Name = "Опис")]
    public string? Info { get; set; }

    [Display(Name = "Дата реєстрації")]
    public DateOnly RegistrationDate { get; set; }


    //public virtual Role Role { get; set; } = null!;
    public virtual ICollection<IdentityUserRole<int>> Roles { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<Registration> Registrations { get; set; }
    public virtual ICollection<Event> Events { get; set; } 
}