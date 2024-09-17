using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class Bank
{
    public decimal Id { get; set; }

    public long? Creditnumber { get; set; }

    public DateTime? Creditexp { get; set; }

    public decimal? Balance { get; set; }

    public decimal? UserId { get; set; }

    public virtual Userr? User { get; set; }
}
