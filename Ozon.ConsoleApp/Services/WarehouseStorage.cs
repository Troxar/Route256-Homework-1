using System.Text.Json;
using Ozon.ConsoleApp.Entities;
using Ozon.ConsoleApp.Exceptions;

namespace Ozon.ConsoleApp.Services;

public interface IWarehouseStorage
{
    void Save(Cell cell);
    IEnumerable<Cell> GetAll();
    public Cell? GetCellByAddressOrDefault(CellAddress requestAddress);
}

internal sealed class WarehouseStorage : IWarehouseStorage
{
    private const string FileName = "cell.json";
    private const string StoragePath = $"{Program.StoragePath}/Warehouse";
    
    private string GetFilePath(CellAddress address)
    {
        return Path.Combine(StoragePath, $"{address.Row}.{address.Shelf}.{address.Rack}.{FileName}");
    }
    
    public void Put(CellAddress address, Product product)
    {
        var cell = GetCellByAddressOrDefault(address); 
        if (cell != null)
            throw new CellIsOccupiedException(address);
        
        Save(new Cell(address, product));
    }
    
    public void Save(Cell cell)
    {
        var path = GetFilePath(cell.Address);
        var directory = Path.GetDirectoryName(path);
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);
        
        var json = JsonSerializer.Serialize(cell, JsonHelper.GetJsonSerializerOptions());
        File.WriteAllText(path, json);
    }
    
    public Cell? GetCellByAddressOrDefault(CellAddress requestAddress)
    {
        var path = GetFilePath(requestAddress);
        
        if (!File.Exists(path))
            return null;
        
        var json = File.ReadAllText(path);
        return TryGetFromJsonOrDefault(json);
    }
    
    public IEnumerable<Cell> GetAll()
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
    
    private static Cell? GetEntity(string path)
    {
        if (!File.Exists(path))
            return null;
        
        var json = File.ReadAllText(path);
        return TryGetFromJsonOrDefault(json);
    }

    private static Cell? TryGetFromJsonOrDefault(string json)
    {
        try
        {
            var result = JsonSerializer.Deserialize<Cell>(json, JsonHelper.GetJsonSerializerOptions());
            return result ?? null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}