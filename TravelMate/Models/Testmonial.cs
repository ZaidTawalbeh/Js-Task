using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class Testmonial
{
    public decimal Id { get; set; }

    public string? Msg { get; set; }

    public string? Status { get; set; }

    public decimal? UserId { get; set; }

    public virtual Userr? User { get; set; }
}
