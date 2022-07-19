using AutoFixture;

namespace Ozon.ConsoleApp.Entities;

public class Cell
{
    public CellAddress Address { get; }
    public Product? Product { get; }
    
    public Cell(CellAddress address, Product? product = null)
    {
        Address = address;
        Product = product;
    }
}