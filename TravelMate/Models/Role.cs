using System;
using System.Collections.Generic;

namespace TravelMate.Models;

public partial class Role
{
    public decimal RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<Userlogin> Userlogins { get; set; } = new List<Userlogin>();
}
