using Game.Shared.Scripts.Entities;
using GameServer.Scripts.Root.ECS;
using Godot;

namespace GameServer.Scripts.Entities;

public partial class PlayerBody : GodotCharacter2D
{
    private AnimatedSprite2D _animatedSprite;
    
    public override void _Ready()
    {
        WorldECS = ServerECS.Instance.World;
        
        base._Ready();
        
    }
}
