using AutoFixture;
using Moq;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;
using System.Collections;

namespace App.Tests;

public class GetProductsFromStorageHandlerTests
{
    private readonly Mock<IWarehouseStorage> _mockWarehouseStorage = GetMockWarehouseStorage();
    
    private static Mock<IWarehouseStorage> GetMockWarehouseStorage()
    {
        int count = 4, id = 0;
        var products = new Fixture()
            .Build<Product>()
            .With(x => x.Id, () => ++id)
            .CreateMany(count)
            .ToArray();
        
        var cells = new List<Cell>
        {
            new (new CellAddress(1, 1, 1), products[0]),
            new (new CellAddress(2, 1, 1), products[1]),
            new (new CellAddress(2, 2, 2), products[2]),
            new (new CellAddress(3, 1, 1), products[3]),
            new (new CellAddress(3, 2, 1), products[2]),
            new (new CellAddress(3, 3, 3), products[1]),
        };
        
        var mock = new Mock<IWarehouseStorage>();
        mock.Setup(x => x.GetAll())
            .Returns(cells);
        
        return mock;
    }
    
    [Theory]
    [ClassData(typeof(DataForTheoryWhenThereAreProductsInRow))]
    public void Handle_WhenThereAreProductsInRow_ShouldReturnProducts(int[] expectedIds, int address)
    {
        //Arrange
        var request = new IGetProductsFromStorageHandler.Request { RequestType = "Row", Address = address };
        var cut = new GetProductsFromStorageHandler(_mockWarehouseStorage.Object);
        
        //Act
        var products = cut.Handle(request).ToArray();

        //Assert
        Assert.Equal(expectedIds.Length, products.Length);
        
        for (int i = 0; i < expectedIds.Length; i++)
            Assert.Equal(expectedIds[i], products[i].Id);
    }
    
    private class DataForTheoryWhenThereAreProductsInRow : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new[] { 1 }, 1 };
            yield return new object[] { new[] { 2, 3 }, 2 };
            yield return new object[] { new[] { 2, 3, 4 }, 3 };
            yield return new object[] { Array.Empty<int>(), 4 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    [Theory]
    [ClassData(typeof(DataForTheoryWhenThereAreProductsOnShelves))]
    public void Handle_WhenThereAreProductsOnShelves_ShouldReturnProducts(int[] expectedIds, int address)
    {
        //Arrange
        var request = new IGetProductsFromStorageHandler.Request { RequestType = "Shelf", Address = address };
        var cut = new GetProductsFromStorageHandler(_mockWarehouseStorage.Object);
        
        //Act
        var products = cut.Handle(request).ToArray();
        
        //Assert
        Assert.Equal(expectedIds.Length, products.Length);
        
        for (int i = 0; i < expectedIds.Length; i++)
            Assert.Equal(expectedIds[i], products[i].Id);
    }
    
    private class DataForTheoryWhenThereAreProductsOnShelves : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new[] { 1, 2, 4 }, 1 };
            yield return new object[] { new[] { 3, 3 }, 2 };
            yield return new object[] { new[] { 2 }, 3 };
            yield return new object[] { Array.Empty<int>(), 4 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    [Fact]
    public void Handle_WhenRequestTypeIsNull_ShouldThrowException()
    {
        //Arrange
        var request = new IGetProductsFromStorageHandler.Request { RequestType = null, Address = 1 };
        var cut = new GetProductsFromStorageHandler(_mockWarehouseStorage.Object);
        
        //Assert
        Assert.Throws<ArgumentException>(() => cut.Handle(request));
    }
    
    [Fact]
    public void Handle_WhenRequestTypeIsWrong_ShouldThrowException()
    {
        //Arrange
        var request = new IGetProductsFromStorageHandler.Request { RequestType = "Rack", Address = 1 };
        var cut = new GetProductsFromStorageHandler(_mockWarehouseStorage.Object);
        
        //Assert
        Assert.Throws<ArgumentException>(() => cut.Handle(request));
    }
    
    [Fact]
    public void Handle_WhenAddressIsNull_ShouldThrowException()
    {
        //Arrange
        var request = new IGetProductsFromStorageHandler.Request { RequestType = "Row", Address = null };
        var cut = new GetProductsFromStorageHandler(_mockWarehouseStorage.Object);
        
        //Assert
        Assert.Throws<ArgumentNullException>(() => cut.Handle(request));
    }
}