using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace GodotFloorLevels.Scripts.Map.Chunk;

/// <summary>
/// Gerenciador central de chunks - responsável por carregar/descarregar chunks
/// Sistema robusto com cache, priorização e geração procedural
/// </summary>
public partial class ChunkManager : Node2D
{
    [Export] public bool Enabled { get; set; } = true; // Ativar/desativar o gerenciador
    [Export] public int ChunkSize { get; set; } = 16;
    [Export] public int TileSize { get; set; } = 32;
    [Export] public int MaxLoadedChunks { get; set; } = 9;
    [Export] public float UnloadDelay { get; set; } = 2.0f;
    [Export] public int NotifierRange { get; set; } = 1; // Raio de notificadores ao redor do player
    [Export] public float ChunkUpdateInterval { get; set; } = 1f; // Intervalo para atualizar prioridades
    [Export] public bool EnableProcGen { get; set; } = true; // Ativar geração procedural
    [Export] public int ProcGenSeed { get; set; } = 12345;
    
    [Export] public NodePath PlayerPath { get; set; } = new("../Player"); // Caminho para o player, se necessário
    
    // Sistemas principais
    private InterfaceAdapters.Godot.Map.Chunk.ChunkCache _chunkCache;
    private ChunkGenerator _chunkGenerator;
    private ChunkPrioritizer _prioritizer;
    
    // Controle de chunks
    private readonly Dictionary<Vector2I, Node2D> _loadedChunks = new();
    private readonly Dictionary<Vector2I, ChunkNotifier> _notifiers = new();
    private readonly Dictionary<Vector2I, SceneTreeTimer> _unloadTimers = new();
    private readonly HashSet<Vector2I> _loadingChunks = []; // Evita carregamento duplo
    
    // Player tracking
    private Node2D _player;
    private Vector2I _lastPlayerChunk = Vector2I.Zero;
    private SceneTreeTimer _updateTimer;
    
    // Performance
    private int _maxChunksPerFrame = 1; // Limita chunks carregados por frame
    private int _chunksLoadedThisFrame = 0;
    
    // Timers
    private double _chunkUpdateTime = 0.0f;
    
    // Sinais para comunicação externa
    [Signal] public delegate void ChunkLoadedEventHandler(Vector2I chunkCoord, Node2D chunkNode, Client.Infrastructure.Godot.Map.Chunk.ChunkData chunkData);
    [Signal] public delegate void ChunkUnloadedEventHandler(Vector2I chunkCoord);
    [Signal] public delegate void ChunkGeneratedEventHandler(Vector2I chunkCoord, Client.Infrastructure.Godot.Map.Chunk.ChunkData chunkData);

    public override void _Ready()
    {
        if (!Enabled)
            return;
        
        InitializeSystems();
        FindPlayer();
    }
    
    public override void _Process(double delta)
    {
        if (!Enabled)
            return;
        
        _chunksLoadedThisFrame = 0; // Reset contador de chunks por frame
        _chunkUpdateTime += delta;  // Acumula tempo para atualizações periódicas
        
        if (_chunkUpdateTime >= ChunkUpdateInterval)
        {
            _chunkUpdateTime = 0.0f; // Reseta o acumulador
            
            if (_player != null)
                OptimizeLoadedChunks();
        }
    }

    private void InitializeSystems()
    {
        // Inicializa sistemas
        _chunkCache = new InterfaceAdapters.Godot.Map.Chunk.ChunkCache();
        _chunkGenerator = new ChunkGenerator();
        _chunkGenerator.UpdateSeed(ProcGenSeed);
        _prioritizer = new ChunkPrioritizer();
        
        GD.Print("ChunkManager inicializado com sistemas robustos");
    }

    private void FindPlayer()
    {
        // Procura pelo player na cena
        _player = GetNodeOrNull<Node2D>(PlayerPath);
        
        if (_player == null)
        {
            GD.PrintErr($"Player não encontrado no caminho: {PlayerPath}. Verifique se o nó existe.");
            return;
        }
        
        UpdatePlayerPosition();
        InitializeNotifiersAroundPlayer();
    }

    private void UpdatePlayerPosition()
    {
        if (_player == null) return;
        
        var currentChunk = WorldToChunkCoord(_player.GlobalPosition);
        _prioritizer.UpdatePlayerPosition(_player.GlobalPosition);
        
        // Se mudou de chunk, atualiza notificadores
        if (currentChunk != _lastPlayerChunk)
        {
            _lastPlayerChunk = currentChunk;
            UpdateNotifiersAroundPlayer();
        }
    }

    private void InitializeNotifiersAroundPlayer()
    {
        if (_player == null) return;
        
        var playerChunk = WorldToChunkCoord(_player.GlobalPosition);
        CreateNotifiersInRange(playerChunk, NotifierRange);
    }

    private void UpdateNotifiersAroundPlayer()
    {
        if (_player == null) return;
        
        var playerChunk = WorldToChunkCoord(_player.GlobalPosition);
        
        // Remove notificadores muito distantes
        var toRemove = _notifiers.Keys.Where(coord => 
            coord.DistanceTo(playerChunk) > NotifierRange + 1).ToList();
        
        foreach (var coord in toRemove)
        {
            RemoveNotifier(coord);
        }
        
        // Adiciona novos notificadores necessários
        CreateNotifiersInRange(playerChunk, NotifierRange);
    }

    private void CreateNotifiersInRange(Vector2I centerChunk, int range)
    {
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                var coord = centerChunk + new Vector2I(x, y);
                CreateNotifier(coord);
                _prioritizer.AddChunk(coord);
            }
        }
    }

    private void InitializeNotifiers()
    {
        // Fallback: cria notificadores em área inicial
        CreateNotifiersInRange(Vector2I.Zero, 1);
    }

    private void CreateNotifier(Vector2I coord)
    {
        if (_notifiers.ContainsKey(coord))
            return;

        var notifier = new ChunkNotifier
        {
            ChunkCoord = coord,
            ChunkSize = ChunkSize,
            TileSize = TileSize
        };

        notifier.ChunkEntered += OnChunkEntered;
        notifier.ChunkExited += OnChunkExited;
        
        GD.Print($"Criando notificador para chunk {coord}");
        
        AddChild(notifier);
        _notifiers[coord] = notifier;
    }

    private void RemoveNotifier(Vector2I coord)
    {
        if (!_notifiers.TryGetValue(coord, out var notifier))
            return;
            
        notifier.ChunkEntered -= OnChunkEntered;
        notifier.ChunkExited -= OnChunkExited;
        notifier.QueueFree();
        _notifiers.Remove(coord);
        _prioritizer.RemoveChunk(coord);
    }

    private async void OnChunkEntered(Vector2I chunkCoord)
    {
        // Cancela timer de unload se existir
        if (_unloadTimers.TryGetValue(chunkCoord, out var timer))
        {
            timer.TimeLeft = 0;
            _unloadTimers.Remove(chunkCoord);
        }

        // Se já está carregado ou carregando, não faz nada
        if (_loadedChunks.ContainsKey(chunkCoord) || _loadingChunks.Contains(chunkCoord))
            return;

        // Verifica limite de chunks por frame
        if (_chunksLoadedThisFrame >= _maxChunksPerFrame)
            return;

        // Carrega o chunk
        await LoadChunk(chunkCoord);
    }

    private void OnChunkExited(Vector2I chunkCoord)
    {
        // Não descarrega imediatamente - usa delay
        if (_unloadTimers.ContainsKey(chunkCoord))
            return;

        var timer = GetTree().CreateTimer(UnloadDelay);
        _unloadTimers[chunkCoord] = timer;
        
        timer.Timeout += () => {
            _unloadTimers.Remove(chunkCoord);
            
            // Verifica se ainda deve descarregar (pode ter mudado prioridade)
            var priority = _prioritizer.GetPriority(chunkCoord);
            if (priority?.ShouldUnload() != false) // Se não tem prioridade ou deve descarregar
            {
                UnloadChunk(chunkCoord);
            }
        };
    }

    private async Task LoadChunk(Vector2I chunkCoord)
    {
        if (!_loadingChunks.Add(chunkCoord))
            return;

        _chunksLoadedThisFrame++;

        try
        {
            // Verifica limite de chunks carregados
            if (_loadedChunks.Count >= MaxLoadedChunks)
                await UnloadLowPriorityChunks();

            // Tenta carregar dados do cache primeiro
            var chunkData = await _chunkCache.GetChunkDataAsync(chunkCoord);
            
            // Se não encontrou e geração procedural está ativa, gera novo chunk
            if (chunkData == null && EnableProcGen)
            {
                chunkData = await _chunkGenerator.GenerateChunkAsync(chunkCoord, ChunkSize);
                await _chunkCache.SaveChunkDataAsync(chunkCoord, chunkData);
                EmitSignal(SignalName.ChunkGenerated, chunkCoord, chunkData);
            }

            // Cria o nó visual do chunk
            var chunkNode = await CreateChunkNode(chunkCoord, chunkData);
            if (chunkNode == null)
                return;

            // Posiciona e adiciona o chunk
            chunkNode.Position = new Vector2(chunkCoord.X * ChunkSize * TileSize, chunkCoord.Y * ChunkSize * TileSize);
            chunkNode.Name = $"Chunk_{chunkCoord.X}_{chunkCoord.Y}";
            
            AddChild(chunkNode);
            _loadedChunks[chunkCoord] = chunkNode;
            
            EmitSignal(SignalName.ChunkLoaded, chunkCoord, chunkNode, chunkData);
            
            GD.Print($"Chunk carregado: {chunkCoord} (Cache: {_chunkCache.IsInMemoryCache(chunkCoord)})");
        }
        finally
        {
            _loadingChunks.Remove(chunkCoord);
        }
    }

    private async Task<Node2D> CreateChunkNode(Vector2I chunkCoord, Client.Infrastructure.Godot.Map.Chunk.ChunkData chunkData)
    {
        if (chunkData == null)
            return null;

        var chunkNode = new Node2D();
        
        // Cria representação visual baseada nos dados
        if (chunkData.TileData.Count > 0)
        {
            var tilemap = new TileMap();
            // Aqui você configuraria o tilemap com base nos dados
            chunkNode.AddChild(tilemap);
        }
        else
        {
            // Fallback: cria visual simples para demonstração
            var visual = new ColorRect
            {
                Size = new Vector2(ChunkSize * TileSize, ChunkSize * TileSize),
                Color = new Color(GD.Randf(), GD.Randf(), GD.Randf(), 0.3f)
            };
            chunkNode.AddChild(visual);
        }
        
        // Adiciona entidades se houver
        foreach (var i in System.Linq.Enumerable.Range(0, chunkData.EntityPositions.Count))
        {
            var entityPos = chunkData.EntityPositions[i];
            var entityType = chunkData.EntityTypes[i];
            
            // Aqui você criaria a entidade baseada no tipo
            var entity = new Node2D();
            entity.Position = new Vector2(entityPos.X * TileSize, entityPos.Y * TileSize);
            entity.Name = entityType;
            chunkNode.AddChild(entity);
        }
        
        // Simula criação assíncrona
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        
        return chunkNode;
    }

    private void OptimizeLoadedChunks()
    {
        // Remove chunks de baixa prioridade se necessário
        if (_loadedChunks.Count > MaxLoadedChunks)
            _ = UnloadLowPriorityChunks();
    }

    private async Task UnloadLowPriorityChunks()
    {
        var chunksToUnload = _prioritizer.GetChunksToUnload(MaxLoadedChunks);
        
        foreach (var coord in chunksToUnload.Take(3)) // Máximo 3 por vez
        {
            UnloadChunk(coord);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
    }

    private void UnloadChunk(Vector2I chunkCoord)
    {
        if (!_loadedChunks.TryGetValue(chunkCoord, out var chunkNode))
            return;

        RemoveChild(chunkNode);
        chunkNode.QueueFree();
        _loadedChunks.Remove(chunkCoord);
        
        EmitSignal(SignalName.ChunkUnloaded, chunkCoord);
        
        GD.Print($"Chunk descarregado: {chunkCoord}");
    }

    // Métodos públicos para controle externo
    public async Task ForceLoadChunk(Vector2I chunkCoord)
    {
        if (!_loadedChunks.ContainsKey(chunkCoord))
        {
            await LoadChunk(chunkCoord);
        }
    }
    
    public void ForceUnloadChunk(Vector2I chunkCoord)
    {
        UnloadChunk(chunkCoord);
    }
    
    public void SetMaxLoadedChunks(int newMax)
    {
        MaxLoadedChunks = newMax;
        OptimizeLoadedChunks();
    }
    
    public void UpdateGeneratorSeed(int newSeed)
    {
        ProcGenSeed = newSeed;
        _chunkGenerator.UpdateSeed(newSeed);
        
        // Limpa cache para forçar regeração
        _chunkCache.ClearMemoryCache();
    }
    
    public Client.Infrastructure.Godot.Map.Chunk.ChunkData GetChunkData(Vector2I chunkCoord)
    {
        // Retorna dados do cache se disponível
        return _chunkCache.IsInMemoryCache(chunkCoord) ? 
            _chunkCache.GetChunkDataAsync(chunkCoord).Result : null;
    }
    
    public List<Vector2I> GetLoadedChunkCoords()
    {
        return _loadedChunks.Keys.ToList();
    }
    
    public List<Vector2I> GetActiveNotifierCoords()
    {
        return _notifiers.Keys.ToList();
    }
    
    public int GetLoadedChunkCount() => _loadedChunks.Count;
    public int GetMemoryCacheSize() => _chunkCache.GetMemoryCacheSize();
    public int GetActiveNotifierCount() => _notifiers.Count;
    
    // Sistema de debug
    public void EnableDebugMode(bool enable = true)
    {
        SetMeta("debug_mode", enable);
        
        if (enable)
        {
            GD.Print("=== CHUNK MANAGER DEBUG ===");
            GD.Print($"Chunks carregados: {GetLoadedChunkCount()}/{MaxLoadedChunks}");
            GD.Print($"Cache em memória: {GetMemoryCacheSize()}");
            GD.Print($"Notificadores ativos: {GetActiveNotifierCount()}");
            GD.Print($"Player chunk: {(_player != null ? WorldToChunkCoord(_player.GlobalPosition) : "N/A")}");
        }
    }
    
    public void PrintChunkStatus()
    {
        GD.Print("=== STATUS DOS CHUNKS ===");
        GD.Print($"Carregados: {string.Join(", ", GetLoadedChunkCoords())}");
        GD.Print($"Notificadores: {string.Join(", ", GetActiveNotifierCoords())}");
        
        if (_player != null)
        {
            var playerChunk = WorldToChunkCoord(_player.GlobalPosition);
            var priorities = _prioritizer.GetChunksByPriority(5);
            GD.Print($"Player em: {playerChunk}");
            GD.Print($"Top 5 prioridades: {string.Join(", ", priorities)}");
        }
    }


    // Métodos utilitários
    public bool IsChunkLoaded(Vector2I chunkCoord) => _loadedChunks.ContainsKey(chunkCoord);
    
    public Node2D GetChunk(Vector2I chunkCoord) => _loadedChunks.GetValueOrDefault(chunkCoord);
    
    public Vector2I WorldToChunkCoord(Vector2 worldPos)
    {
        var chunkWorldSize = ChunkSize * TileSize;
        return new Vector2I(
            Mathf.FloorToInt(worldPos.X / chunkWorldSize),
            Mathf.FloorToInt(worldPos.Y / chunkWorldSize)
        );
    }

    public override void _ExitTree()
    {
        // Limpa todos os chunks carregados
        foreach (var chunk in _loadedChunks.Values)
        {
            chunk?.QueueFree();
        }
        _loadedChunks.Clear();
        
        // Limpa timers
        foreach (var timer in _unloadTimers.Values)
        {
            timer.TimeLeft = 0;
        }
        _unloadTimers.Clear();
        
        base._ExitTree();
    }
}