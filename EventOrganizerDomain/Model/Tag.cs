using System;
using System.Collections.Generic;

namespace EventOrganizerDomain.Model;

public partial class Tag : Entity
{
    //public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
