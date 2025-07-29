using Arch.Core;
using Game.Shared.Resources;
using Game.Shared.Scripts.Entities;
using Game.Shared.Scripts.Entities.Data;
using Godot;
using GodotFloorLevels.Scripts.Root.ECS;

namespace GodotFloorLevels.Scripts.Entities;

public partial class PlayerBody : GodotCharacter2D
{
    private AnimatedSprite2D _animatedSprite;
    
    public override void _Ready()
    {
        WorldECS = ClientECS.Instance.World;
        
        base._Ready();
        
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        //_animatedSprite.SpriteFrames = 
            //ResourcePaths.Resources.SpriteFramesArcherMale.Load();
        
        _animatedSprite.Play();
    }

    public static PlayerBody CreatePlayer(PlayerResource playerResource, World world)
    {
        var scene = ResourcePaths.Scenes.Entities.PlayerBody.Load().Instantiate<PlayerBody>();
        scene.PlayerResource = playerResource;
        scene.WorldECS = world;
        return scene;
    }
}
