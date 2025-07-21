using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using Godot;

namespace GodotFloorLevels.Scripts.Map.Chunk;

/// <summary>
/// Visualizador de debug para o sistema de chunks
/// Mostra informações visuais sobre chunks carregados, cache, performance, etc.
/// </summary>
public partial class ChunkDebugVisualizer : Node2D
{
    [Export] public bool Enabled { get; set; } = true; // Ativar/desativar o gerenciador
    [Export] public bool ShowDebugInfo { get; set; } = false;
    [Export] public bool ShowChunkBorders { get; set; } = false;
    [Export] public bool ShowPerformanceStats { get; set; } = false;
    
    private ChunkManager _chunkManager;
    private Label _debugLabel;
    private Control _debugPanel;
    private RichTextLabel _performanceLabel;
    
    // Performance tracking
    private float _frameTime = 0.0f;
    private int _chunksLoadedThisSecond = 0;
    private float _timeAccumulator = 0.0f;
    
    public override void _Ready()
    {
        if (!Enabled)
            return;
        
        CreateDebugUI();
        FindChunkManager();
    }
    
    private void CreateDebugUI()
    {
        // Painel principal de debug
        _debugPanel = new Control();
        _debugPanel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.TopLeft);
        _debugPanel.Position = new Vector2(10, 10);
        _debugPanel.Visible = ShowDebugInfo;
        AddChild(_debugPanel);
        
        // Background do painel
        var background = new NinePatchRect();
        background.Texture = CreateDebugBackground();
        background.Size = new Vector2(300, 400);
        _debugPanel.AddChild(background);
        
        // Label principal de debug
        _debugLabel = new Label();
        _debugLabel.Position = new Vector2(10, 10);
        _debugLabel.Size = new Vector2(280, 200);
        _debugLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _debugPanel.AddChild(_debugLabel);
        
        // Label de performance
        _performanceLabel = new RichTextLabel();
        _performanceLabel.Position = new Vector2(10, 220);
        _performanceLabel.Size = new Vector2(280, 170);
        _performanceLabel.FitContent = true;
        _debugPanel.AddChild(_performanceLabel);
    }
    
    private ImageTexture CreateDebugBackground()
    {
        var image = Image.CreateEmpty(1, 1, false, Image.Format.Rgba8);
        image.Fill(new Color(0, 0, 0, 0.7f));
        var texture = ImageTexture.CreateFromImage(image);
        return texture;
    }
    
    private void FindChunkManager()
    {
        _chunkManager = GetTree().GetFirstNodeInGroup("chunk_manager") as ChunkManager;
        if (_chunkManager == null)
        {
            _chunkManager = GetNode<ChunkManager>("../ChunkManager");
        }
        
        if (_chunkManager != null)
        {
            _chunkManager.ChunkLoaded += OnChunkLoaded;
            _chunkManager.ChunkUnloaded += OnChunkUnloaded;
        }
    }
    
    public override void _Process(double delta)
    {
        if (!Enabled)
            return;
        
        if (!ShowDebugInfo || _chunkManager == null)
            return;
            
        _frameTime = (float)delta;
        _timeAccumulator += _frameTime;
        
        if (_timeAccumulator >= 1.0f)
        {
            _timeAccumulator = 0.0f;
            _chunksLoadedThisSecond = 0;
        }
        
        UpdateDebugInfo();
        
        if (ShowPerformanceStats)
        {
            UpdatePerformanceStats();
        }
    }
    
    private void UpdateDebugInfo()
    {
        if (_debugLabel == null || _chunkManager == null)
            return;
            
        var loadedChunks = _chunkManager.GetLoadedChunkCoords();
        var notifiers = _chunkManager.GetActiveNotifierCoords();
        
        var debugText = $"=== CHUNK SYSTEM DEBUG ===\n";
        debugText += $"Chunks Carregados: {_chunkManager.GetLoadedChunkCount()}/{_chunkManager.MaxLoadedChunks}\n";
        debugText += $"Cache em Memória: {_chunkManager.GetMemoryCacheSize()}\n";
        debugText += $"Notificadores Ativos: {_chunkManager.GetActiveNotifierCount()}\n";
        debugText += $"Chunks/Segundo: {_chunksLoadedThisSecond}\n\n";
        
        debugText += $"Chunks Carregados:\n";
        foreach (var chunk in loadedChunks.Take(5))
        {
            debugText += $"  {chunk},";
        }
        
        if (loadedChunks.Count > 5)
        {
            debugText += $"  ... e mais {loadedChunks.Count - 5}\n";
        }
        
        _debugLabel.Text = debugText;
    }
    
    private void UpdatePerformanceStats()
    {
        if (_performanceLabel == null)
            return;
            
        var fps = Engine.GetFramesPerSecond();
        var memoryUsage = OS.GetStaticMemoryUsage();
        
        _performanceLabel.Clear(); // Limpa o conteúdo anterior
        
        _performanceLabel.Text = $"[color=yellow]=== PERFORMANCE ===[/color]\n";

        _performanceLabel.AppendText($"FPS: [color=green]{fps:F1}[/color]\n");
        _performanceLabel.AppendText($"Frame Time: [color=cyan]{_frameTime * 1000:F2}ms[/color]\n");
        _performanceLabel.AppendText($"Memory: [color=orange]{memoryUsage / 1024 / 1024:F1}MB[/color]\n\n");
        
        
        // Estatísticas do sistema de chunks
        if (_chunkManager != null)
        {
            _performanceLabel.AppendText($"[color=yellow]=== CHUNK STATS ===[/color]\n");
            _performanceLabel.AppendText($"Update Interval: {_chunkManager.ChunkUpdateInterval}s\n");
            _performanceLabel.AppendText($"Unload Delay: {_chunkManager.UnloadDelay}s\n");
            _performanceLabel.AppendText($"Notifier Range: {_chunkManager.NotifierRange}\n");
            _performanceLabel.AppendText($"Chunk Size: {_chunkManager.ChunkSize}x{_chunkManager.ChunkSize}\n");
            _performanceLabel.AppendText($"Tile Size: {_chunkManager.TileSize}px\n");
        }
    }
    
    public override void _Draw()
    {
        if (!Enabled)
            return;
        
        if (!ShowChunkBorders || _chunkManager == null)
            return;
            
        DrawChunkBorders();
    }
    
    private void DrawChunkBorders()
    {
        var loadedChunks = _chunkManager.GetLoadedChunkCoords();
        var chunkWorldSize = _chunkManager.ChunkSize * _chunkManager.TileSize;
        
        foreach (var coord in loadedChunks)
        {
            var worldPos = new Vector2(coord.X * chunkWorldSize, coord.Y * chunkWorldSize);
            var rect = new Rect2(worldPos, new Vector2(chunkWorldSize, chunkWorldSize));
            
            // Desenha borda do chunk
            DrawRect(rect, Colors.Green, false, 2.0f);
            
            // Desenha coordenada do chunk
            var font = ThemeDB.FallbackFont;
            var text = $"{coord.X},{coord.Y}";
            DrawString(font, worldPos + new Vector2(5, 20), text, HorizontalAlignment.Left, -1, 16, Colors.White);
        }
    }
    
    private void OnChunkLoaded(Vector2I chunkCoord, Node2D chunkNode, Client.Infrastructure.Godot.Map.Chunk.ChunkData chunkData)
    {
        _chunksLoadedThisSecond++;
        GD.Print($"[DEBUG] Chunk {chunkCoord} carregado");
    }
    
    private void OnChunkUnloaded(Vector2I chunkCoord)
    {
        GD.Print($"[DEBUG] Chunk {chunkCoord} descarregado");
    }
    
    // Métodos públicos para controle
    public void ToggleDebugInfo()
    {
        ShowDebugInfo = !ShowDebugInfo;
        if (_debugPanel != null)
        {
            _debugPanel.Visible = ShowDebugInfo;
        }
    }
    
    public void ToggleChunkBorders()
    {
        ShowChunkBorders = !ShowChunkBorders;
        QueueRedraw();
    }
    
    public void TogglePerformanceStats()
    {
        ShowPerformanceStats = !ShowPerformanceStats;
        if (_performanceLabel != null)
        {
            _performanceLabel.Visible = ShowPerformanceStats;
        }
    }
    
    public void LogChunkStatus()
    {
        _chunkManager?.PrintChunkStatus();
    }
    
    // Input handling para debug
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true } keyEvent)
        {
            switch (keyEvent.Keycode)
            {
                case Key.F1:
                    ToggleDebugInfo();
                    break;
                case Key.F2:
                    ToggleChunkBorders();
                    break;
                case Key.F3:
                    TogglePerformanceStats();
                    break;
                case Key.F4:
                    LogChunkStatus();
                    break;
            }
        }
    }
    
    public override void _ExitTree()
    {
        if (_chunkManager != null)
        {
            _chunkManager.ChunkLoaded -= OnChunkLoaded;
            _chunkManager.ChunkUnloaded -= OnChunkUnloaded;
        }
        base._ExitTree();
    }
}
