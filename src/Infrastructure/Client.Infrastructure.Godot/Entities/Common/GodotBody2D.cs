using Client.Infrastructure.Godot.Resources;
using Godot;

namespace Client.Infrastructure.Godot.Entities.Common;

/// <summary>
/// Entidade base para todos os objetos do jogo (Jogadores, NPCs, etc.),
/// estendendo CharacterBody2D para aproveitar física e movimento nativo do Godot.
/// Obtém o World via nó autoload configurado no Godot (Project Settings > Autoload).
/// </summary>
public partial class GodotBody2D : CharacterBody2D
{
    // Resources
    [Export] protected GodotSpriteResource GodotSpriteResource = null!; // Componente de sprite
    [Export] protected GodotMovement GodotMovement = null!; // Componente de movimento
    [Export] protected GodotTransform GodotTransform = null!; // Componente de transformação
    
    public Guid Id { get; } = Guid.NewGuid(); // ID único para cada instância

    public override void _Ready()
    {
        base._Ready();
        
        InitializeSprite();
    }

    private void InitializeSprite()
    {
        // Adiciona o sprite como filho do nó atual
        AddChild(GodotSpriteResource);
    }

}