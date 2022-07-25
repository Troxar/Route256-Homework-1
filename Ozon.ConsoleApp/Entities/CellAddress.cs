namespace Ozon.ConsoleApp.Entities;

public class CellAddress
{
    public int Row { get; }
    public int Shelf { get; }
    public int Rack { get; }
    
    public CellAddress(int row, int shelf, int rack)
    {
        Row = row;
        Shelf = shelf;
        Rack = rack;
    }

    public override string ToString()
    {
        return $"{Row}-{Shelf}-{Rack}";
    }
}