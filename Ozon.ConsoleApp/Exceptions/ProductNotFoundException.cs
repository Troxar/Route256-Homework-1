namespace Ozon.ConsoleApp.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(int id) : base($"Product not found by id: {id}") { }
}