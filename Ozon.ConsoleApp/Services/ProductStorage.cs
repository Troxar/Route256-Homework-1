using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Ozon.ConsoleApp.Entities;

namespace Ozon.ConsoleApp.Services;

public interface IProductStorage
{
    IEnumerable<Product> GetAll();
    Product? GetProductByIdOrDefault(int requestId);
}

internal sealed class ProductStorage : IProductStorage
{
    private const string FileName = "product.json";
    private const string StoragePath = $"{Program.StoragePath}/Products";
    
    private string GetFilePath(int id)
    {
        return Path.Combine(StoragePath, $"{id}.{FileName}");
    }
    
    public IEnumerable<Product> GetAll()
    {
        if (!Directory.Exists(StoragePath))
            yield break;

        foreach (var file in Directory.GetFiles(StoragePath))
        {
            var entity = GetEntity(file);
            if (entity != null)
                yield return entity;
        }
    }

    private static Product? GetEntity(string path)
    {
        if (!File.Exists(path))
            return null;

        var json = File.ReadAllText(path);
        return TryGetFromJsonOrDefault(json);
    }

    private static Product? TryGetFromJsonOrDefault(string json)
    {
        try
        {
            var result = JsonSerializer.Deserialize<Product>(json, JsonHelper.GetJsonSerializerOptions());
            return result ?? null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
    
    public Product? GetProductByIdOrDefault(int requestId)
    {
        var path = GetFilePath(requestId);
        
        if (!File.Exists(path))
            return null;
        
        var json = File.ReadAllText(path);
        return TryGetFromJsonOrDefault(json);
    }
}