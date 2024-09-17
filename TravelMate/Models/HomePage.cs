using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelMate.Models;

public partial class HomePage
{
    public decimal Id { get; set; }

    public string? Imagelogo { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }
    public string? PWelcome { get; set; }

    public string? Imagemain { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile2 { get; set; }
    public string? PFooter { get; set; }

    public string? PCopyrigth { get; set; }
}
