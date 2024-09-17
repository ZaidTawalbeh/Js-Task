using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMate.Models;

public partial class Userr
{
    public decimal UserId { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public decimal? Phone { get; set; }

    public string? Emale { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }
    public virtual ICollection<Bank> Banks { get; set; } = new List<Bank>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Testmonial> Testmonials { get; set; } = new List<Testmonial>();

    public virtual ICollection<Userlogin> Userlogins { get; set; } = new List<Userlogin>();
}
