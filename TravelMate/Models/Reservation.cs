using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class Reservation
{
    public decimal Id { get; set; }

    public DateTime? Checkindate { get; set; }

    public DateTime? Checkoutdate { get; set; }

    public decimal? UserId { get; set; }

    public decimal? RoomId { get; set; }

    public virtual Room? Room { get; set; }

    public virtual Userr? User { get; set; }
}
