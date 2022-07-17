using Moq;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;

namespace App.Tests;

public class GetProductsHandlerTests
{
    private readonly Mock<IProductStorage> _mockProductStorage;

    public GetProductsHandlerTests()
    {
        var productList = new List<Product>
        {
            new ("p01"),
            //new ("p02") { IsSponsored = true },
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
    
    [Fact]
    public void Handle_WhenPassChildren_ShouldGetProductsInTheRightOrder()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Joe", Gender.Male, 5, "Soccer"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p03", result[0].Name);
        Assert.Equal("p04", result[1].Name);
        Assert.Equal("p05", result[2].Name);
        Assert.Equal("p13", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassTeenagerUnder18_ShouldGetProductsInTheRightOrder()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Amy", Gender.Female, 17, "Music"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p04", result[0].Name);
        Assert.Equal("p03", result[1].Name);
        Assert.Equal("p08", result[2].Name);
        Assert.Equal("p14", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassTeenagerAbove18_ShouldGetProductsInTheRightOrder()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Amy", Gender.Female, 19, "Soccer"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p10", result[0].Name);
        Assert.Equal("p12", result[1].Name);
        Assert.Equal("p09", result[2].Name);
        Assert.Equal("p11", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassAdult_ShouldGetProductsInTheRightOrder()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Bender", Gender.NotDecide, 40, "Soccer"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p09", result[0].Name);
        Assert.Equal("p10", result[1].Name);
        Assert.Equal("p11", result[2].Name);
        Assert.Equal("p12", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassOld_ShouldGetProductsInTheRightOrder()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Jim", Gender.Male, 60, "Soccer"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p11", result[0].Name);
        Assert.Equal("p09", result[1].Name);
        Assert.Equal("p12", result[2].Name);
        Assert.Equal("p10", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassAge0_ShouldGetProductsLikeForChildren()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Joe", Gender.Male, 0, "Soccer"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p03", result[0].Name);
        Assert.Equal("p04", result[1].Name);
        Assert.Equal("p05", result[2].Name);
        Assert.Equal("p13", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassAge15_ShouldGetProductsLikeForTeenager()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Amy", Gender.Female, 17, "Music"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p04", result[0].Name);
        Assert.Equal("p03", result[1].Name);
        Assert.Equal("p08", result[2].Name);
        Assert.Equal("p14", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassAge21_ShouldGetProductsLikeForAdult()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Bender", Gender.NotDecide, 21, "Soccer"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p09", result[0].Name);
        Assert.Equal("p10", result[1].Name);
        Assert.Equal("p11", result[2].Name);
        Assert.Equal("p12", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenPassAge55_ShouldGetProductsLikeForOld()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        mockClientStorage.Setup(x => x.GetClientByNameOrDefault(It.IsAny<string>()))
            .Returns(new Client("Jim", Gender.Male, 55, "Soccer"));
        var request = new IGetProductsHandler.Request { ClientName = "_" }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Act
        var result = cut.Handle(request).ToArray();

        //Assert
        Assert.True(result.Length >= 4);
        Assert.Equal("p11", result[0].Name);
        Assert.Equal("p09", result[1].Name);
        Assert.Equal("p12", result[2].Name);
        Assert.Equal("p10", result[3].Name);
    }
    
    [Fact]
    public void Handle_WhenClientNameIsNull_ShouldThrowException()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        var request = new IGetProductsHandler.Request { ClientName = null }; 
        var cut = new GetProductsHandler(_mockProductStorage.Object, mockClientStorage.Object);
        
        //Assert
        Assert.Throws<ArgumentException>(() => cut.Handle(request));
    }
    
    [Fact]
    public void Handle_WhenClientNameIsEmpty_ShouldThrowException()
    {
        // Arrange
        var mockClientStorage = new Mock<IClientStorage>();
        var request = new IGetProductsHandler.Request { ClientName = "" }; 
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