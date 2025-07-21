using Client.Infrastructure.Godot.Resources;
using Godot;
using Godot.Collections;

namespace Client.Infrastructure.Godot.Map.Chunk;

/// <summary>
/// Classe para armazenar dados do chunk de forma serializada
/// </summary>
[System.Serializable]
public partial class ChunkData : Resource
{
    [Export] public Vector2I ChunkCoord { get; set; } = Vector2I.Zero;
    [Export] public int ChunkSize { get; set; } = 16;
    [Export] public Array<int> TileData { get; set; } = [];
    [Export] public Array<Vector2I> EntityPositions { get; set; } = [];
    [Export] public Array<string> EntityTypes { get; set; } = [];
    [Export] public bool IsGenerated { get; set; } = false;
    [Export] public GodotDataResource LastAccessed { get; set; } = new();
    
    public ChunkData()
    {
    }
    
    public ChunkData(Vector2I coord, int size)
    {
        ChunkCoord = coord;
        ChunkSize = size;
        TileData = [];
        EntityPositions = [];
        EntityTypes = [];
    }
    
    public void SetTile(int x, int y, int tileId)
    {
        if (x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize)
            return;
            
        int index = y * ChunkSize + x;
        while (TileData.Count <= index)
            TileData.Add(0);
            
        TileData[index] = tileId;
    }
    
    public int GetTile(int x, int y)
    {
        if (x < 0 || x >= ChunkSize || y < 0 || y >= ChunkSize)
            return 0;
            
        int index = y * ChunkSize + x;
        return index < TileData.Count ? TileData[index] : 0;
    }
    
    public void AddEntity(Vector2I position, string entityType)
    {
        EntityPositions.Add(position);
        EntityTypes.Add(entityType);
    }
    
    public void UpdateLastAccessed()
    {
        LastAccessed.FromDateTime(DateTime.Now);
    }
}
