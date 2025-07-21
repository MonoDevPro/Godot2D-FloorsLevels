using Godot;
using GodotFloorLevels.Scripts.Map.Chunk;
using ChunkData = Client.Infrastructure.Godot.Map.Chunk.ChunkData;

namespace GodotFloorLevels.Scripts.Map;

/// <summary>
/// Exemplo de uso do sistema de chunks
/// Este script demonstra como configurar e usar o ChunkManager de forma eficiente
/// </summary>
public partial class ChunkSystemExample : Node2D
{
    [Export] public bool Enabled { get; set; } = true; // Ativar/desativar o gerenciador
    [Export] public Chunk.ChunkManager ChunkManager { get; set; }
    [Export] public ChunkDebugVisualizer DebugVisualizer { get; set; }
    
    public override void _Ready()
    {
        if (!Enabled)
            return;
        
        SetupChunkSystem();
        ConnectSignals();
    }
    
    private void SetupChunkSystem()
    {
        // Configuração básica do ChunkManager
        if (ChunkManager != null)
        {
            // Ativa modo debug
            ChunkManager.EnableDebugMode(true);
        }
    }
    
    private void ConnectSignals()
    {
        if (ChunkManager == null) return;
        
        // Conecta aos sinais do ChunkManager
        ChunkManager.ChunkLoaded += OnChunkLoaded;
        ChunkManager.ChunkUnloaded += OnChunkUnloaded;
        ChunkManager.ChunkGenerated += OnChunkGenerated;
    }
    
    private void OnChunkLoaded(Vector2I chunkCoord, Node2D chunkNode, ChunkData chunkData)
    {
        GD.Print($"[EXEMPLO] Chunk {chunkCoord} foi carregado com {chunkData.EntityPositions.Count} entidades");
        
        // Aqui você pode adicionar lógica específica quando um chunk é carregado
        // Por exemplo: ativar sistemas de IA, spawnar inimigos, etc.
    }
    
    private void OnChunkUnloaded(Vector2I chunkCoord)
    {
        GD.Print($"[EXEMPLO] Chunk {chunkCoord} foi descarregado");
        
        // Aqui você pode salvar dados importantes antes do chunk ser removido
    }
    
    private void OnChunkGenerated(Vector2I chunkCoord, ChunkData chunkData)
    {
        GD.Print($"[EXEMPLO] Novo chunk {chunkCoord} foi gerado com {chunkData.TileData.Count} tiles");
        
        // Aqui você pode modificar o chunk recém-gerado
        // Por exemplo: adicionar estruturas especiais, recursos raros, etc.
    }
    
    // Exemplo de uso da API pública do ChunkManager
    public async void ExampleUsage()
    {
        if (ChunkManager == null) return;
        
        // Força carregar um chunk específico
        await ChunkManager.ForceLoadChunk(new Vector2I(5, 5));
        
        // Verifica se um chunk está carregado
        bool isLoaded = ChunkManager.IsChunkLoaded(new Vector2I(0, 0));
        GD.Print($"Chunk (0,0) está carregado: {isLoaded}");
        
        // Obtém dados de um chunk
        var chunkData = ChunkManager.GetChunkData(new Vector2I(0, 0));
        if (chunkData != null)
        {
            GD.Print($"Chunk (0,0) tem {chunkData.EntityPositions.Count} entidades");
        }
        
        // Altera configurações em tempo real
        ChunkManager.SetMaxLoadedChunks(16); // Aumenta limite para 16 chunks
        ChunkManager.UpdateGeneratorSeed(54321); // Muda seed da geração
        
        // Obtém estatísticas
        GD.Print($"Chunks carregados: {ChunkManager.GetLoadedChunkCount()}");
        GD.Print($"Cache em memória: {ChunkManager.GetMemoryCacheSize()}");
        GD.Print($"Notificadores ativos: {ChunkManager.GetActiveNotifierCount()}");
    }
    
    public override void _Input(InputEvent @event)
    {
        if (!Enabled)
            return;
        
        if (@event is InputEventKey { Pressed: true } keyEvent)
        {
            switch (keyEvent.Keycode)
            {
                case Key.F5:
                    // Executa exemplo de uso
                    ExampleUsage();
                    break;
                    
                case Key.F6:
                    // Imprime status dos chunks
                    ChunkManager?.PrintChunkStatus();
                    break;
                    
                case Key.F7:
                    // Regenera mundo com nova seed
                    if (ChunkManager != null)
                    {
                        var newSeed = GD.RandRange(1000, 99999);
                        ChunkManager.UpdateGeneratorSeed(newSeed);
                        GD.Print($"Nova seed aplicada: {newSeed}");
                    }
                    break;
            }
        }
    }
    
    public override void _ExitTree()
    {
        // Desconecta sinais
        if (ChunkManager != null)
        {
            ChunkManager.ChunkLoaded -= OnChunkLoaded;
            ChunkManager.ChunkUnloaded -= OnChunkUnloaded;
            ChunkManager.ChunkGenerated -= OnChunkGenerated;
        }
        
        base._ExitTree();
    }
}
