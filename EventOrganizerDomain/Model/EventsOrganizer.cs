using System;
using System.Collections.Generic;

namespace EventOrganizerDomain.Model;

public partial class EventsOrganizer : Entity
{
    public int OrganizerId { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User Organizer { get; set; } = null!;
}
