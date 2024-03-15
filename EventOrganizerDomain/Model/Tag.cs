using System;
using System.Collections.Generic;

namespace EventOrganizerDomain.Model;

public partial class Tag : Entity
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
}
