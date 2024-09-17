using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMate.Models;

public partial class AboutPage
{
    public decimal Id { get; set; }

    public string? Imagemain { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }
    public string? PAbout { get; set; }
}
