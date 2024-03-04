using System;
using System.Collections.Generic;

namespace EventOrganizerDomain.Model;

public partial class Country : Entity
{
    //public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
