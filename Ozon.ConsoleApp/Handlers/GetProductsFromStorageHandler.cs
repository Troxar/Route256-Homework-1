using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp.Handlers;

internal interface IGetProductsFromStorageHandler
{
    public class Request
    {
        [Display(Name = "request-type")]
        public string? RequestType { get; set; }
        
        [Display(Name = "address")]
        public int? Address { get; set; }
    }
    
    ICollection<Product> Handle(Request request);
}

[UsedImplicitly]
internal sealed class GetProductsFromStorageHandler : IGetProductsFromStorageHandler
{
    private readonly IWarehouseStorage _warehouseStorage;
    
    public GetProductsFromStorageHandler(IWarehouseStorage warehouseStorage)
    {
        _warehouseStorage = warehouseStorage;
    }
    
    public ICollection<Product> Handle(IGetProductsFromStorageHandler.Request request)
    {
        if (!Enum.TryParse<WarehouseRequestType>(request.RequestType, out var requestType))
            throw new ArgumentException(request.RequestType, nameof(request.RequestType));
        
        ArgumentNullException.ThrowIfNull(request.Address);
        
        IEnumerable<Cell> cells = _warehouseStorage.GetAll();
        
        return cells
            .Where(x => CompareUsingRequestType(x, requestType, (int)request.Address))
            .Select(x => x.Product)
            .OrderBy((x => x.Id))
            .ToArray();
    }

    private bool CompareUsingRequestType(Cell cell, WarehouseRequestType requestType, int address)
    {
        switch (requestType)
        {
            case WarehouseRequestType.Row: return cell.Address.Row == address;
            case WarehouseRequestType.Shelf: return cell.Address.Shelf == address;
            default: return false;
        }
    }
}