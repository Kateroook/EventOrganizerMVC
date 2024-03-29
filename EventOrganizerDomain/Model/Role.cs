﻿using System;
using System.Collections.Generic;

namespace EventOrganizerDomain.Model;

public partial class Role : Entity
{

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
