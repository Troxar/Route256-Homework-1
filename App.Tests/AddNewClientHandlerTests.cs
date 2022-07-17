using AutoFixture;
using Moq;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Handlers;
using Ozon.ConsoleApp.Services;

namespace App.Tests;

public class AddNewClientHandlerTests
{
    private Gender GetRandomGender()
    {
        Random random = new();
        var maxValue= Enum.GetValues(typeof(Gender)).Length;
        return (Gender)random.Next(0, maxValue);
    }
    
    [Fact]
    public void Handle_WhenPassName_ShouldSaveClient()
    {
        // Arrange
        var request = new Fixture().Build<IAddNewClientHandler.Request>()
            .With(x => x.Gender, GetRandomGender().ToString())
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