using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace EventOrganizerDomain.Model;

public partial class Place : Entity
{
    //public int Id { get; set; }

    public int PlaceTypeId { get; set; }

    public int? CityId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? UnitNumber { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? Zip { get; set; }

    public Geometry? CoordinatesCol1 { get; set; }

    public string? CoordinatesCol2 { get; set; }

    public byte[]? PlaceImage { get; set; }

    public int? Capacity { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ContactEmail { get; set; }

    public string? Website { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual PlaceType PlaceType { get; set; } = null!;
}
