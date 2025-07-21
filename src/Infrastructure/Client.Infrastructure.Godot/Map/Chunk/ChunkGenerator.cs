using System.Threading.Tasks;
using Godot;

namespace GodotFloorLevels.Scripts.Map.Chunk;

/// <summary>
/// Gerador procedural de chunks com diferentes biomas e características
/// </summary>
public partial class ChunkGenerator : RefCounted
{
    [Export] public int Seed { get; set; } = 12345;
    [Export] public float NoiseScale { get; set; } = 0.1f;
    [Export] public int OctaveCount { get; set; } = 4;
    [Export] public float Persistence { get; set; } = 0.5f;
    [Export] public float Lacunarity { get; set; } = 2.0f;
    
    private FastNoiseLite _terrainNoise;
    private FastNoiseLite _biomeNoise;
    private FastNoiseLite _resourceNoise;
    
    public ChunkGenerator()
    {
        InitializeNoise();
    }
    
    private void InitializeNoise()
    {
        _terrainNoise = new FastNoiseLite();
        _terrainNoise.Seed = Seed;
        _terrainNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        _terrainNoise.Frequency = NoiseScale;
        _terrainNoise.FractalOctaves = OctaveCount;
        _terrainNoise.FractalLacunarity = Lacunarity;
        _terrainNoise.FractalGain = Persistence;
        
        _biomeNoise = new FastNoiseLite();
        _biomeNoise.Seed = Seed + 1000;
        _biomeNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
        _biomeNoise.Frequency = NoiseScale * 0.5f;
        
        _resourceNoise = new FastNoiseLite();
        _resourceNoise.Seed = Seed + 2000;
        _resourceNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular;
        _resourceNoise.Frequency = NoiseScale * 2.0f;
    }
    
    public async Task<Client.Infrastructure.Godot.Map.Chunk.ChunkData> GenerateChunkAsync(Vector2I chunkCoord, int chunkSize)
    {
        var chunkData = new Client.Infrastructure.Godot.Map.Chunk.ChunkData(chunkCoord, chunkSize);
        
        // Simula geração assíncrona para não travar o jogo
        await ToSignal(Engine.GetMainLoop(), SceneTree.SignalName.ProcessFrame);
        
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                var worldX = chunkCoord.X * chunkSize + x;
                var worldY = chunkCoord.Y * chunkSize + y;
                
                var tileId = GenerateTileAtPosition(worldX, worldY);
                chunkData.SetTile(x, y, tileId);
                
                // Gera entidades ocasionalmente
                if (ShouldPlaceEntity(worldX, worldY))
                {
                    var entityType = GetEntityTypeForPosition(worldX, worldY);
                    chunkData.AddEntity(new Vector2I(x, y), entityType);
                }
            }
            
            // Yield ocasionalmente para manter responsividade
            if (x % 4 == 0)
            {
                await ToSignal(Engine.GetMainLoop(), SceneTree.SignalName.ProcessFrame);
            }
        }
        
        chunkData.IsGenerated = true;
        return chunkData;
    }
    
    private int GenerateTileAtPosition(int worldX, int worldY)
    {
        var terrainValue = _terrainNoise.GetNoise2D(worldX, worldY);
        var biomeValue = _biomeNoise.GetNoise2D(worldX, worldY);
        
        // Determina bioma baseado no ruído
        var biome = GetBiomeFromNoise(biomeValue);
        
        // Determina tipo de terreno baseado no valor de ruído
        return GetTileIdForTerrain(terrainValue, biome);
    }
    
    private BiomeType GetBiomeFromNoise(float noiseValue)
    {
        if (noiseValue < -0.3f) return BiomeType.Ocean;
        if (noiseValue < 0.0f) return BiomeType.Beach;
        if (noiseValue < 0.3f) return BiomeType.Grassland;
        if (noiseValue < 0.6f) return BiomeType.Forest;
        return BiomeType.Mountain;
    }
    
    private int GetTileIdForTerrain(float terrainValue, BiomeType biome)
    {
        return biome switch
        {
            BiomeType.Ocean => terrainValue < -0.5f ? 1 : 2,      // Água profunda/rasa
            BiomeType.Beach => 3,                                  // Areia
            BiomeType.Grassland => terrainValue > 0.2f ? 5 : 4,   // Grama alta/baixa
            BiomeType.Forest => terrainValue > 0.4f ? 7 : 6,      // Floresta densa/normal
            BiomeType.Mountain => terrainValue > 0.6f ? 9 : 8,    // Rocha/pedra
            _ => 4 // Default: grama
        };
    }
    
    private bool ShouldPlaceEntity(int worldX, int worldY)
    {
        var resourceValue = _resourceNoise.GetNoise2D(worldX, worldY);
        return resourceValue > 0.7f; // 30% de chance aproximadamente
    }
    
    private string GetEntityTypeForPosition(int worldX, int worldY)
    {
        var biomeValue = _biomeNoise.GetNoise2D(worldX, worldY);
        var biome = GetBiomeFromNoise(biomeValue);
        
        return biome switch
        {
            BiomeType.Forest => "Tree",
            BiomeType.Mountain => "Rock",
            BiomeType.Grassland => "Flower",
            BiomeType.Beach => "Shell",
            BiomeType.Ocean => "Seaweed",
            _ => "Grass"
        };
    }
    
    public void UpdateSeed(int newSeed)
    {
        Seed = newSeed;
        InitializeNoise();
    }
}

public enum BiomeType
{
    Ocean,
    Beach,
    Grassland,
    Forest,
    Mountain
}
