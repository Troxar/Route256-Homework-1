using System.ComponentModel.DataAnnotations;

namespace Ozon.ConsoleApp.Services;

internal class GetProductsFromStorageRequest
{
    [Display(Name = "request-type")]
    public string? RequestType { get; set; }
    
    [Display(Name = "address")]
    public int? Address { get; set; }
}