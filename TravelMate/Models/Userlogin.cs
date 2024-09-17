using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class Userlogin
{
    public decimal Id { get; set; }

    public string? Username { get; set; }

    public string? Passwordd { get; set; }

    public decimal? UserId { get; set; }

    public decimal? RoleId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual Userr? User { get; set; }
}
