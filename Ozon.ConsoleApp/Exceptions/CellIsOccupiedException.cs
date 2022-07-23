using Ozon.ConsoleApp.Entities;

namespace Ozon.ConsoleApp.Exceptions;

public class CellIsOccupiedException : Exception
{
    public CellIsOccupiedException(CellAddress address) : base($"Cell is occupied: {address}") { }
}