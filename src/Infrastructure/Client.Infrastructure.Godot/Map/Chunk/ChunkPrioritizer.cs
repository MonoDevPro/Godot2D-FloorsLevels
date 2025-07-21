using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace GodotFloorLevels.Scripts.Map.Chunk;

/// <summary>
/// Sistema de priorização de chunks baseado em distância do player e importância
/// </summary>
public partial class ChunkPrioritizer : RefCounted
{
    private Vector2 _playerPosition = Vector2.Zero;
    private readonly Dictionary<Vector2I, ChunkPriority> _chunkPriorities = new();
    
    public void UpdatePlayerPosition(Vector2 position)
    {
        _playerPosition = position;
        RecalculatePriorities();
    }
    
    public void AddChunk(Vector2I chunkCoord, ChunkPriority priority = null)
    {
        _chunkPriorities[chunkCoord] = priority ?? CalculatePriority(chunkCoord);
    }
    
    public void RemoveChunk(Vector2I chunkCoord)
    {
        _chunkPriorities.Remove(chunkCoord);
    }
    
    public List<Vector2I> GetChunksByPriority(int maxCount = -1)
    {
        var sortedChunks = _chunkPriorities
            .OrderByDescending(kvp => kvp.Value.Score)
            .Select(kvp => kvp.Key);
            
        if (maxCount > 0)
            return sortedChunks.Take(maxCount).ToList();
            
        return sortedChunks.ToList();
    }
    
    public List<Vector2I> GetChunksToUnload(int keepCount)
    {
        if (_chunkPriorities.Count <= keepCount)
            return new List<Vector2I>();
            
        return _chunkPriorities
            .OrderBy(kvp => kvp.Value.Score)
            .Take(_chunkPriorities.Count - keepCount)
            .Select(kvp => kvp.Key)
            .ToList();
    }
    
    public ChunkPriority GetPriority(Vector2I chunkCoord)
    {
        return _chunkPriorities.GetValueOrDefault(chunkCoord);
    }
    
    private void RecalculatePriorities()
    {
        var chunksToUpdate = _chunkPriorities.Keys.ToList();
        foreach (var coord in chunksToUpdate)
        {
            _chunkPriorities[coord] = CalculatePriority(coord);
        }
    }
    
    private ChunkPriority CalculatePriority(Vector2I chunkCoord)
    {
        // Converte coordenada do chunk para posição do mundo
        var chunkWorldPos = new Vector2(chunkCoord.X * 16 * 32, chunkCoord.Y * 16 * 32); // ChunkSize * TileSize
        var distanceToPlayer = _playerPosition.DistanceTo(chunkWorldPos);
        
        // Calcula prioridade baseada na distância (mais próximo = maior prioridade)
        float distanceScore = Math.Max(0, 1000f - distanceToPlayer);
        
        // Bônus para chunks centrais (ao redor do player)
        float centralBonus = 0f;
        if (distanceToPlayer < 512f) // 1 chunk de distância
            centralBonus = 500f;
        else if (distanceToPlayer < 1024f) // 2 chunks de distância
            centralBonus = 200f;
        
        // Penalidade para chunks muito distantes
        float distancePenalty = distanceToPlayer > 2048f ? -1000f : 0f;
        
        var priority = new ChunkPriority
        {
            ChunkCoord = chunkCoord,
            DistanceToPlayer = distanceToPlayer,
            Score = distanceScore + centralBonus + distancePenalty,
            LastUpdated = DateTime.Now
        };
        
        return priority;
    }
    
    public void Clear()
    {
        _chunkPriorities.Clear();
    }
}

/// <summary>
/// Dados de prioridade para um chunk específico
/// </summary>
public class ChunkPriority
{
    public Vector2I ChunkCoord { get; set; }
    public float DistanceToPlayer { get; set; }
    public float Score { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsImportant { get; set; } = false; // Para chunks com conteúdo especial
    
    public bool ShouldUnload()
    {
        return Score < -500f && !IsImportant;
    }
}
