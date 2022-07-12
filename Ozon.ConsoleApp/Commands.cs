using System.CommandLine;
using Ozon.ConsoleApp.Handlers;

namespace Ozon.ConsoleApp;

public class Commands
{
    public static IEnumerable<Command> CreateRootCommands(CancellationTokenSource cts)
    {
        yield return CreateExitCommand(cts);
        yield return CreateUpdateProducts();
    }
    
    private static Command CreateUpdateProducts()
    {
        var customerCommand = new Command("update-products");
        var option = new Option<int>("count");
        customerCommand.AddOption(option);
        
        customerCommand.SetHandler(x =>
        {
            var handler = new GenerateProductsHandler();
            handler.Handle(new GenerateProductsHandler.Request(x));
            Console.WriteLine("Success!");
        }, option);
        
        return customerCommand;
    }
    
    private static Command CreateExitCommand(CancellationTokenSource cts)
    {
        var customerCommand = new Command("exit", "exit from program");
        customerCommand.SetHandler(x => cts.Cancel());
        return customerCommand;
    }
}