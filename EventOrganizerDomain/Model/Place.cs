using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace EventOrganizerDomain.Model;

public partial class Place : Entity
{
    [Display(Name="Тип місця")]
    public int PlaceTypeId { get; set; }

    [Display(Name="Місто")]
    public int? CityId { get; set; }

    [Display(Name="Назва")]
    public string Name { get; set; } = null!;

    [Display(Name="Опис")]
    public string? Description { get; set; }

    [Display(Name="Номер")]
    public string? UnitNumber { get; set; }

    [Display(Name="Вулиця")]
    public string? AddressLine1 { get; set; }

    [Display(Name="Вулиця")]
    public string? AddressLine2 { get; set; }

    [Display(Name="Поштовий індекс")]
    public string? Zip { get; set; }

    [Display(Name="Адреса")]
    public string Address
    {
        get
        {
            string address = AddressLine1 ?? "";
            if (!string.IsNullOrEmpty(AddressLine2))
            {
                address += ", " + AddressLine2;
            }
            if (!string.IsNullOrEmpty(UnitNumber))
            {
                address += ", " + UnitNumber;
            }
            return address;
        }
    }
    public Geometry? CoordinatesCol1 { get; set; }

    public string? CoordinatesCol2 { get; set; }

    [Display(Name="Кількість місць")]
    public int? Capacity { get; set; }

    [Display(Name="Телефон")] 
    public string? PhoneNumber { get; set; }

    [Display(Name="Пошта")] 
    public string? ContactEmail { get; set; }

    [Display(Name="Вебсайт")] 
    public string? Website { get; set; }

    [Display(Name="Посилання на постер")]
    public string? ImageUrl { get; set; }


    public virtual City? City { get; set; }
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual PlaceType PlaceType { get; set; } = null!;
}