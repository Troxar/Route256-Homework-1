using JetBrains.Annotations;
using Ozon.ConsoleApp.Abstractions;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Exceptions;
using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp.Handlers;

[UsedImplicitly]
internal sealed class PutProductHandler : IPutProductHandler
{
    private readonly IProductStorage _productStorage;
    private readonly IWarehouseStorage _warehouseStorage;

    public PutProductHandler(
        IProductStorage productStorage,
        IWarehouseStorage warehouseStorage)
    {
        _productStorage = productStorage;
        _warehouseStorage = warehouseStorage;
    }

    public void Handle(PutProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request.ProductId);
        ArgumentNullException.ThrowIfNull(request.Row);
        ArgumentNullException.ThrowIfNull(request.Shelf);
        ArgumentNullException.ThrowIfNull(request.Rack);

        var productId = (int)request.ProductId;
        var product = _productStorage.GetProductByIdOrDefault(productId);
        if (product == null)
            throw new ProductNotFoundException(productId);
        
        var address = new CellAddress((int)request.Row, (int)request.Shelf, (int)request.Rack);
        var cell = _warehouseStorage.GetCellByAddressOrDefault(address); 
        if (cell != null)
            throw new CellIsOccupiedException(address);
        
        _warehouseStorage.Save(new Cell(address, product));
    }
}