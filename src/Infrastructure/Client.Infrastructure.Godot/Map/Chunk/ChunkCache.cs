using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using GodotFloorLevels.Scripts.Map.Chunk;
using ChunkData = Client.Infrastructure.Godot.Map.Chunk.ChunkData;

namespace GodotFloorLevels.Scripts.InterfaceAdapters.Godot.Map.Chunk;

/// <summary>
/// Sistema de cache para chunks com persistência em disco
/// </summary>
public partial class ChunkCache : GodotObject
{
    private readonly Dictionary<Vector2I, ChunkData> _memoryCache = new();
    private readonly Dictionary<Vector2I, DateTime> _accessTimes = new();
    private readonly string _cacheDirectory;
    private readonly int _maxMemoryCacheSize;
    
    public ChunkCache() : this("user://chunks", 25)
    {
    }
    
    public ChunkCache(string cacheDir = "user://chunks", int maxMemorySize = 25)
    {
        _cacheDirectory = cacheDir;
        _maxMemoryCacheSize = maxMemorySize;
        
        // Cria diretório de cache se não existir
        var baseDir = DirAccess.Open("user://");
        if (baseDir != null && !baseDir.DirExists("chunks"))
            baseDir.MakeDir("chunks");
    }
    
    public async Task<ChunkData> GetChunkDataAsync(Vector2I coord)
    {
        // Verifica cache em memória primeiro
        if (_memoryCache.TryGetValue(coord, out var cachedData))
        {
            _accessTimes[coord] = DateTime.Now;
            cachedData.UpdateLastAccessed();
            return cachedData;
        }
        
        // Tenta carregar do disco
        var chunkData = await LoadFromDiskAsync(coord);
        if (chunkData != null)
        {
            AddToMemoryCache(coord, chunkData);
            return chunkData;
        }
        
        return null;
    }
    
    public async Task SaveChunkDataAsync(Vector2I coord, ChunkData data)
    {
        // Salva em memória
        AddToMemoryCache(coord, data);
        
        // Salva em disco de forma assíncrona
        await SaveToDiskAsync(coord, data);
    }
    
    private void AddToMemoryCache(Vector2I coord, ChunkData data)
    {
        // Remove chunks antigos se necessário
        if (_memoryCache.Count >= _maxMemoryCacheSize)
        {
            CleanOldestFromMemory();
        }
        
        _memoryCache[coord] = data;
        _accessTimes[coord] = DateTime.Now;
        data.UpdateLastAccessed();
    }
    
    private void CleanOldestFromMemory()
    {
        if (_accessTimes.Count == 0) return;
        
        var oldest = _accessTimes.OrderBy(kvp => kvp.Value).First();
        _memoryCache.Remove(oldest.Key);
        _accessTimes.Remove(oldest.Key);
    }
    
    private async Task<ChunkData> LoadFromDiskAsync(Vector2I coord)
    {
        string filePath = GetChunkFilePath(coord);
        
        GD.Print(OS.GetUserDataDir());
        
        if (!global::Godot.FileAccess.FileExists(filePath))
            return null;
        
        try
        {
            // Executa I/O em background para não bloquear a thread principal
            return await Task.Run(() =>
            {
                var file = global::Godot.FileAccess.Open(filePath, global::Godot.FileAccess.ModeFlags.Read);
                if (file == null) return null;
                
                var jsonString = file.GetAsText();
                file.Close();
                
                var json = new Json();
                var parseResult = json.Parse(jsonString);
                
                if (parseResult != Error.Ok)
                    return null;
                
                var dict = json.Data.AsGodotDictionary();
                return DictToChunkData(dict);
            });
        }
        catch (Exception e)
        {
            GD.PrintErr($"Erro ao carregar chunk {coord}: {e.Message}");
            return null;
        }
    }
    
    private async Task SaveToDiskAsync(Vector2I coord, ChunkData data)
    {
        string filePath = GetChunkFilePath(coord);
        
        try
        {
            // Executa I/O em background para não bloquear a thread principal
            await Task.Run(() =>
            {
                var dict = ChunkDataToDict(data);
                var jsonString = Json.Stringify(dict);
                
                var file = global::Godot.FileAccess.Open(filePath, global::Godot.FileAccess.ModeFlags.Write);
                if (file == null) return;
                
                file.StoreString(jsonString);
                file.Close();
            });
        }
        catch (Exception e)
        {
            GD.PrintErr($"Erro ao salvar chunk {coord}: {e.Message}");
        }
    }
    
    private string GetChunkFilePath(Vector2I coord)
    {
        return $"{_cacheDirectory}/chunk_{coord.X}_{coord.Y}.json";
    }
    
    private global::Godot.Collections.Dictionary ChunkDataToDict(ChunkData data)
    {
        return new global::Godot.Collections.Dictionary
        {
            ["chunk_coord_x"] = data.ChunkCoord.X,
            ["chunk_coord_y"] = data.ChunkCoord.Y,
            ["chunk_size"] = data.ChunkSize,
            ["tile_data"] = data.TileData,
            ["entity_positions"] = data.EntityPositions,
            ["entity_types"] = data.EntityTypes,
            ["is_generated"] = data.IsGenerated,
            ["last_accessed"] = data.LastAccessed.ToDateTime().ToBinary()
        };
    }
    
    private ChunkData DictToChunkData(global::Godot.Collections.Dictionary dict)
    {
        var data = new ChunkData();
        data.ChunkCoord = new Vector2I(dict["chunk_coord_x"].AsInt32(), dict["chunk_coord_y"].AsInt32());
        data.ChunkSize = dict["chunk_size"].AsInt32();
        data.TileData = dict["tile_data"].AsGodotArray<int>();
        data.EntityPositions = dict["entity_positions"].AsGodotArray<Vector2I>();
        data.EntityTypes = dict["entity_types"].AsGodotArray<string>();
        data.IsGenerated = dict["is_generated"].AsBool();
        data.LastAccessed.FromDateTime(DateTime.FromBinary(dict["last_accessed"].AsInt64()));
        return data;
    }
    
    public void ClearMemoryCache()
    {
        _memoryCache.Clear();
        _accessTimes.Clear();
    }
    
    public bool IsInMemoryCache(Vector2I coord)
    {
        return _memoryCache.ContainsKey(coord);
    }
    
    public int GetMemoryCacheSize()
    {
        return _memoryCache.Count;
    }
}
