using Ozon.ConsoleApp.Services;

namespace Ozon.ConsoleApp.Abstractions;

internal interface IPutProductHandler
{
    void Handle(PutProductRequest request);
}