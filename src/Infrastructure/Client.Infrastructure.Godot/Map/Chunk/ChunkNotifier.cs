using Godot;

namespace GodotFloorLevels.Scripts.Map.Chunk;

/// <summary>
/// ChunkNotifier apenas detecta visibilidade e emite sinais - não gerencia instâncias
/// </summary>
[Tool]
public partial class ChunkNotifier : VisibleOnScreenNotifier2D
{
    [Export(PropertyHint.Range, "0,1000,1")]
    public Vector2I ChunkCoord {
        get => _chunkCoord;
        set {
            _chunkCoord = value;
            SetupNotifier();
        }
    }
    
    [Export] public int ChunkSize { get; set; } = 16;
    [Export] public int TileSize { get; set; } = 32;
    [Export] public NodePath ChunkPath { get; set; }
    
    [Signal] public delegate void ChunkEnteredEventHandler(Vector2I chunkCoord);
    [Signal] public delegate void ChunkExitedEventHandler(Vector2I chunkCoord);
    
    private Vector2I _chunkCoord;
    private Node2D _chunkNode;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            ScreenEntered += OnScreenEntered;
            ScreenExited += OnScreenExited;
        }
        
        SetupNotifier();
    }
    
    private void LoadChunk()
    {
        if (ChunkPath.IsEmpty)
        {
            GD.PrintErr(
                "ChunkPath não está definido. Por favor, defina o caminho do chunk.",
                "Adicionando um ColorRect no lugar do chunk.");
            
            var colorRect = new ColorRect
            {
                Color = new Color(1, 0, 0, 0.5f), // Vermelho semi-transparente
                Size = Rect.Size,
            };
            
            AddChild(colorRect);
            
            ChunkPath = colorRect.GetPath();
        }
        
        _chunkNode = GetNode<Node2D>(ChunkPath);
        
        if (_chunkNode == null)
        {
            GD.PrintErr($"Não foi possível encontrar o nó no caminho: {ChunkPath}");
            return;
        }
        
        // Configura o tamanho do nó do chunk
        _chunkNode.Position = _chunkCoord * ChunkSize * TileSize;
        //_chunkNode.RectSize = new Vector2(ChunkSize * TileSize, ChunkSize * TileSize);
    }

    private void SetupNotifier()
    {
        // Posiciona o notificador
        var pos = _chunkCoord * ChunkSize * TileSize;
        var size = new Vector2(ChunkSize * TileSize, ChunkSize * TileSize);
        
        Position = pos;
        Rect = new Rect2(0, 0, size);
        
        
    }

    private void OnScreenEntered()
    {
        EmitSignal(SignalName.ChunkEntered, ChunkCoord);
        GD.Print($"Chunk {ChunkCoord} entrou na tela.");
    }

    private void OnScreenExited()
    {
        EmitSignal(SignalName.ChunkExited, ChunkCoord);
        GD.Print($"Chunk {ChunkCoord} saiu da tela.");
    }

    public override void _ExitTree()
    {
        if (!Engine.IsEditorHint())
        {
            ScreenEntered -= OnScreenEntered;
            ScreenExited -= OnScreenExited;
        }
        
        base._ExitTree();
    }
}