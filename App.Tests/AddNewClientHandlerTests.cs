using AutoFixture;
using Moq;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;

namespace App.Tests;

public class AddNewClientHandlerTests
{
    [Fact]
    public void Handle_WhenPassName_ShouldSaveClient()
    {
        // Arrange
        Random random = new();
        var request = new Fixture().Build<IAddNewClientHandler.Request>()
            .With(x => x.Gender, ((Gender)random.Next(0, 3)).ToString())
            .Create();
        
        var mock = new Mock<IClientStorage>();
        var cut = new AddNewClientHandler(mock.Object);
        
        //Act
        cut.Handle(request);

        //Assert
        mock.Verify(x => x.Save(It.Is<Client>(x => x.Name == request.Name)));
        mock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void Handle_WhenNoPassName_ShouldThrow()
    {
        // Arrange
        var mock = new Mock<IClientStorage>();
        var cut = new AddNewClientHandler(mock.Object);
        
        //Act
        Assert.Throws<ArgumentException>(() => cut.Handle(new IAddNewClientHandler.Request() { Name = string.Empty }));
    }
}