using Godot;

namespace GodotFloorLevels.Scripts.Map;

public partial class MapLayer : CanvasLayer
{
    [Export]
    public Node2D PlayerNode;

    [Export]
    public Node2D CameraNode;

    [Export]
    public Node2D MapRoot;

    public override void _Ready()
    {
        if (PlayerNode == null)
            GD.PrintErr("PlayerNode não está definido no MapLayer.");

        if (CameraNode == null)
            GD.PrintErr("CameraNode não está definido no MapLayer.");

        if (MapRoot == null)
            GD.PrintErr("MapRoot não está definido no MapLayer.");
    }
}