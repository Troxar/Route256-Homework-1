﻿using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Exceptions;
using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp.Handlers;

internal interface IPutProductHandler
{
    void Handle(IPutProductHandler.Request request);
    
    public class Request
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
}

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

    public void Handle(IPutProductHandler.Request request)
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