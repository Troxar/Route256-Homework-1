using System.ComponentModel.DataAnnotations;

namespace Ozon.ConsoleApp.Services;

internal class PutProductRequest
{
    [Display(Name = "product-id")]
    public int? ProductId { get; set; }

    [Display(Name = "row")]
    public int? Row { get; set;  }

    [Display(Name = "shelf")]
    public int? Shelf { get; set;  }

    [Display(Name = "rack")]
    public int? Rack { get; set;  }
}