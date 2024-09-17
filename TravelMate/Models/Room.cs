using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class Room
{
    public decimal RoomId { get; set; }

    public string? Roomnumber { get; set; }

    public string? Roomtype { get; set; }

    public decimal? Pricepernight { get; set; }

    public string? Isavailable { get; set; }

    public decimal? HotelId { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
