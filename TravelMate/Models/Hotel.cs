using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMate.Models;

public partial class Hotel
{
    public decimal HotelId { get; set; }

    public string? Hotelname { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
