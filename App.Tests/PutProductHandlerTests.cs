using AutoFixture;
using Moq;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Exceptions;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;

namespace App.Tests;

public class PutProductHandlerTests
{
    private readonly Mock<IProductStorage> _mockProductStorage = new ();
    private readonly Mock<IWarehouseStorage> _mockWarehouseStorage = new ();
    
    [Fact]
    public void Handle_WhenProductIdIsOkAndCellIsFree_ShouldSaveCell()
    {
        //Arrange
        var request = new IPutProductHandler.Request { ProductId = 1, Row = 1, Shelf = 1, Rack = 1 };
        _mockProductStorage.Setup(x => x.GetProductByIdOrDefault(It.IsAny<int>()))
            .Returns(new Product("p1"));
        _mockWarehouseStorage.Setup(x => x.GetCellByAddressOrDefault(It.IsAny<CellAddress>()))
            .Returns((Cell?)null);
        var cut = new PutProductHandler(_mockProductStorage.Object, _mockWarehouseStorage.Object);
        
        //Act
        cut.Handle(request);

        //Assert
        _mockWarehouseStorage.Verify(x => x.Save(It.IsAny<Cell>()));
    }
    
    [Fact]
    public void Handle_WhenCellIsOccupied_ShouldThrowException()
    {
        //Arrange
        var request = new IPutProductHandler.Request { ProductId = 1, Row = 1, Shelf = 1, Rack = 1 };
        _mockProductStorage.Setup(x => x.GetProductByIdOrDefault(It.IsAny<int>()))
            .Returns(new Product("p1"));
        _mockWarehouseStorage.Setup(x => x.GetCellByAddressOrDefault(It.IsAny<CellAddress>()))
            .Returns(new Fixture().Build<Cell>().Create());
        var cut = new PutProductHandler(_mockProductStorage.Object, _mockWarehouseStorage.Object);
        
        //Assert
        Assert.Throws<CellIsOccupiedException>(() => cut.Handle(request));
    }
    
    [Fact]
    public void Handle_WhenProductIdIsWrong_ShouldThrowException()
    {
        //Arrange
        var request = new IPutProductHandler.Request { ProductId = 1, Row = 1, Shelf = 1, Rack = 1 };
        _mockProductStorage.Setup(x => x.GetProductByIdOrDefault(It.IsAny<int>()))
            .Returns((Product?)null);
        var cut = new PutProductHandler(_mockProductStorage.Object, _mockWarehouseStorage.Object);
        
        //Assert
        Assert.Throws<ProductNotFoundException>(() => cut.Handle(request));
    }
    
    [Theory]
    [InlineData(null, 1, 1, 1)]
    [InlineData(1, null, 1, 1)]
    [InlineData(1, 1, null, 1)]
    [InlineData(1, 1, 1, null)]
    public void Handle_WhenRequestParameterIsNull_ShouldThrowException(int? productId, int? row, int? shelf, int? rack)
    {
        //Arrange
        var request = new IPutProductHandler.Request { ProductId = productId, Row = row, Shelf = shelf, Rack = rack };
        var cut = new PutProductHandler(_mockProductStorage.Object, _mockWarehouseStorage.Object);
        
        //Assert
        Assert.Throws<ArgumentNullException>(() => cut.Handle(request));
    }
}