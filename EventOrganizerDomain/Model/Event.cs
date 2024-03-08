using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventOrganizerDomain.Model;

public partial class Event : Entity
{
    public Event()
    {
        Tags = new HashSet<Tag>();
    }
    public int PlaceId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Speaker { get; set; }
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy, HH:mm, ddd}", ApplyFormatInEditMode = false)]   
    public DateTime DateTimeStart { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd MMMM, HH:mm, ddd}", ApplyFormatInEditMode = false)]
    public DateTime DateTimeEnd { get; set; }

    public double? Price { get; set; }

    public string? Capacity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }
    public string? PictureUrl { get; set; }




    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Place Place { get; set; } = null!;

    public virtual ICollection<Registration> Registrations { get; set; }
    public virtual ICollection<User> Organizers { get; set; } = new List<User>();

    public virtual ICollection<Tag> Tags { get; set; }


}
