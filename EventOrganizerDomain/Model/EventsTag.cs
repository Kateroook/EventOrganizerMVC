using System;
using System.Collections.Generic;

namespace EventOrganizerDomain.Model;

public partial class EventsTag : Entity
{
    public int TagId { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
