using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp.Abstractions;

internal interface IGetProductsFromStorageHandler
{
    ICollection<Product> Handle(GetProductsFromStorageRequest request);
}