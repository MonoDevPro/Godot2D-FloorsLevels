using Godot;

namespace Client.Infrastructure.Godot.Resources;

[GlobalClass]
public partial class GodotTransform(Vector2 position, float rotationRadians, Vector2 scale) : Resource
{
    [Export] public Vector2 Position { get; set; } = position;
    [Export] public float RotationRadians { get; set; } = rotationRadians;
    [Export] public Vector2 Scale { get; set; } = scale;

    public GodotTransform() : this(Vector2.Zero, 0f, Vector2.One)
    {
    }

    /// <summary>
    /// Constrói o Transform2D com escala, rotação e deslocamento.
    /// </summary>
    public Transform2D ToTransform2D()
    {
        var t = Transform2D.Identity
            .Rotated(RotationRadians)
            .Scaled(Scale);
        t.Origin = Position;
        return t;
    }

    // Implicit conversion para Transform2D
    public static implicit operator Transform2D(GodotTransform t) => t.ToTransform2D();
}