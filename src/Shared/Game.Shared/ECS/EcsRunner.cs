using Arch.Core;
using Arch.System;
using Game.Shared.ECS.Components;
using Godot;

namespace Game.Shared.ECS;

public abstract partial class EcsRunner : Node
{
    public World World { get; private set; }
    
    private readonly Group<float> _deltaGroup = new("DeltaGroup");

    public override void _Ready()
    {
        World = World.Create(); // Create a new ECS world
        
        OnCreateSystems([]);
        
        _deltaGroup.Initialize(); // Initialize the delta group
        
        GD.Print("[Arch ECS] Mundo ECS criado");
    }

    protected virtual void OnCreateSystems(List<ISystem<float>> systems)
    {
        _deltaGroup.Add(systems.ToArray());
    }
    
    public override void _Process(double delta)
    {
        var deltaF = (float)delta;
        _deltaGroup.BeforeUpdate(deltaF); // Call before update hook
        _deltaGroup.Update(deltaF);        // Update all systems
        _deltaGroup.AfterUpdate(deltaF);   // Call after update hook
    }
    
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public void RpcSendInput(string peerId, Vector2 input)
    {
        // Find entity by peerId and apply InputCommandComponent
        var entity = /* lookup by RemoteTag(PeerId) */;
        World.Set();  (entity, new InputCommandComponent { Input = input });
    }
    
    public override void _ExitTree()
    {
        _deltaGroup.Dispose();              // Dispose of the ECS systems
        World.Dispose();                    // Dispose of the ECS world
        
        GD.Print("ECS Runner exited and resources cleaned up.");
    }
}
