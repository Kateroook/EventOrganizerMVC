using System;
using System.Collections.Generic;

namespace EventOrganizerDomain.Model;

public partial class Tag : Entity
{
    //public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? AddedAt { get; set; }
}
