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

    [Required(ErrorMessage = "Оберіть місце проведення!")]
    [Display(Name = "Місце")]
    public int PlaceId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Display(Name = "Доповідач")]
    public string? Speaker { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy, HH:mm, ddd}", ApplyFormatInEditMode = false)]
    public DateTime DateTimeStart { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd MMMM, HH:mm, ddd}", ApplyFormatInEditMode = false)]
    public DateTime DateTimeEnd { get; set; }

    [NotMapped]
    [DataType(DataType.Date)]
    public string EventDate
    {
        get
        {
            // Проверяем, если день начала и день конца совпадают
            if (DateTimeStart.Date == DateTimeEnd.Date)
            {
                return $"{DateTimeStart:dd MMMM yyyy, ddd}";
            }
            else
            {               
                return $"{DateTimeStart:dd MMMM yyyy} - {DateTimeEnd:dd MMMM yyyy}";
            }
        }
    }    

    [NotMapped]
    [DataType(DataType.Time)]
    public string EventTime
    {
        get
        {
            // Проверяем, если день начала и день конца совпадают
            if (DateTimeStart.Date == DateTimeEnd.Date)
            {                
                return $"{DateTimeStart:HH:mm} - {DateTimeEnd:HH:mm}";
            }
            else
            {
                
                return $"{DateTimeStart:HH:mm}";
            }
        }
    }



    [Display(Name ="Ціна (грн)")]
    public double? Price { get; set; }

    [Display(Name ="Кількість місць")]
    public int? Capacity { get; set; }

    [Display(Name ="Створено")]
    public DateTime CreatedAt { get; set; }

    [Display(Name ="Оновлено")]
    public DateTime LastUpdatedAt { get; set; }

    [Display(Name = "URL зображення")]
    [DataType(DataType.ImageUrl)]
    [RegularExpression(@"\b(?:https?://|www\.)\S+\b", ErrorMessage = "Неправильний формат URL")]
    public string? PictureUrl { get; set; }


    public virtual Place Place { get; set; } = null!;
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public virtual ICollection<User> Organizers { get; set; } = new List<User>();
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}