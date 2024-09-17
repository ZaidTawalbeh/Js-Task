using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class Service
{
    public decimal Id { get; set; }

    public string? Servicename { get; set; }

    public string? Servicedescription { get; set; }
}
