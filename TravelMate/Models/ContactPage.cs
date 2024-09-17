using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class ContactPage
{
    public decimal Id { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Location { get; set; }
}
