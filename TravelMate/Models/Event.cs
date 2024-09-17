using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMate.Models;

public partial class Event
{
    public decimal Id { get; set; }

    public string? Eventname { get; set; }

    public string? Eventdescription { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }
    public decimal? HotelId { get; set; }

    public virtual Hotel? Hotel { get; set; }
}
