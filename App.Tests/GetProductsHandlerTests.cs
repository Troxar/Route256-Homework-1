using Moq;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;
using System.Collections;

namespace App.Tests;

public class GetProductsHandlerTests
{
    private readonly Mock<IProductStorage> _mockProductStorage;

    public GetProductsHandlerTests()
    {
        var productList = new List<Product>
        {
            new ("p01"),
            new ("p03") { IsSponsored = true, Gender = Gender.Male },
            new ("p04") { IsSponsored = true, Gender = Gender.Female },
            new ("p05") { Age = Ages.Children, Gender = Gender.Male },
            new ("p06") { Age = Ages.Children, Gender = Gender.Female },
            new ("p07") { Age = Ages.Teenager, Gender = Gender.Male },
            new ("p08") { Age = Ages.Teenager, Gender = Gender.Female },
            new ("p09") { Age = Ages.Adult, Gender = Gender.Male, Adult = true },
            new ("p10") { Age = Ages.Adult, Gender = Gender.Female, Adult = true },
            new ("p11") { Age = Ages.Old, Gender = Gender.Male, Adult = true },
            new ("p12") { Age = Ages.Old, Gender = Gender.Female, Adult = true },
            new ("p13") { Gender = Gender.Male, Tags = new[] { "Soccer" } },
            new ("p14") { Gender = Gender.Female, Tags = new[] { "Music" } },
        };
        
        _mockProductStorage = new Mock<IProductStorage>();
        _mockProductStorage.Setup(x => x.GetAll())
            .Returns(productList);
    }
    
    [Theory]
    [ClassData(typeof(DataForTheoryWhenPassClientShouldGetAppropriateOrderOfProducts))]
    public void Handle_WhenPassClient_ShouldGetAppropriateOrderOfProducts(Client client, string[] expectedNames)
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(client);
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.Equal(expectedNames.Length, result.Length);
        
        for (int i = 0; i < result.Length; i++)
            Assert.Equal(expectedNames[i], result[i].Name);
    }
    
    private class DataForTheoryWhenPassClientShouldGetAppropriateOrderOfProducts : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Client("Joe", Gender.Male, 0, "Soccer"),
                new [] { "p03", "p04", "p05", "p13", "p06", "p07", "p01", "p08", "p14" }
            };
            yield return new object[]
            {
                new Client("Joe", Gender.Male, 5, "Soccer"),
                new [] { "p03", "p04", "p05", "p13", "p06", "p07", "p01", "p08", "p14" }
            };
            yield return new object[]
            {
                new Client("Amy", Gender.Female, 15, "Music"),
                new [] { "p04", "p03", "p08", "p14", "p06", "p07", "p01", "p05", "p13" }
            };
            yield return new object[]
            {
                new Client("Amy", Gender.Female, 17, "Music"),
                new [] { "p04", "p03", "p08", "p14", "p06", "p07", "p01", "p05", "p13" }
            };
            yield return new object[]
            {
                new Client("Amy", Gender.Female, 19, "Soccer"),
                new [] { "p10", "p12", "p09", "p11" }
            };
            yield return new object[]
            {
                new Client("Bender", Gender.NotDecide, 21, "Soccer"),
                new [] { "p09", "p10", "p11", "p12" }
            };
            yield return new object[]
            {
                new Client("Bender", Gender.NotDecide, 40, "Soccer"),
                new [] { "p09", "p10", "p11", "p12" }
            };
            yield return new object[]
            {
                new Client("Jim", Gender.Male, 55, "Soccer"),
                new [] { "p11", "p09", "p12", "p10" }
            };
            yield return new object[]
            {
                new Client("Jim", Gender.Male, 60, "Soccer"),
                new [] { "p11", "p09", "p12", "p10" }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Handle_WhenClientNameIsNullOrEmpty_ShouldThrowException(string? name)
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        var request = new IGetProductsHandler.Request { ClientName = name }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Assert
        Assert.Throws<ArgumentException>(() => cut.Handle(request));
    }
    
    [Fact]
    public void Handle_WhenStorageDoesNotContainClient_ShouldThrowException()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns((Client)null);
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Assert
        Assert.Throws<Exception>(() => cut.Handle(request));
    }
}