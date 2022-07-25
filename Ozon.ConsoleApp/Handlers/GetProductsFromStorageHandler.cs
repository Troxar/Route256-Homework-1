using JetBrains.Annotations;
using Ozon.ConsoleApp.Abstractions;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp.Handlers;

[UsedImplicitly]
internal sealed class GetProductsFromStorageHandler : IGetProductsFromStorageHandler
{
    private readonly IWarehouseStorage _warehouseStorage;
    
    public GetProductsFromStorageHandler(IWarehouseStorage warehouseStorage)
    {
        _warehouseStorage = warehouseStorage;
    }
    
    public ICollection<Product> Handle(GetProductsFromStorageRequest request)
    {
        if (!Enum.TryParse<WarehouseRequestType>(request.RequestType, out var requestType))
            throw new ArgumentOutOfRangeException(nameof(request.RequestType), request.RequestType);
        
        ArgumentNullException.ThrowIfNull(request.Address);
        
        IEnumerable<Cell> cells = _warehouseStorage.GetAll();
        
        return cells
            .Where(x => CompareUsingRequestType(x, requestType, (int)request.Address))
            .Select(x => x.Product)
            .OrderBy((x => x.Id))
            .ToArray();
    }

    private bool CompareUsingRequestType(Cell cell, WarehouseRequestType requestType, int address)
        => requestType switch
    {
        WarehouseRequestType.Row => cell.Address.Row == address,
        WarehouseRequestType.Shelf => cell.Address.Shelf == address,
        _ => false
    };
}