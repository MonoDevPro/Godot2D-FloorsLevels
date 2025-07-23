using Client.Application.Abstraction.Boundary;
using Godot;

namespace Client.Infrastructure.Godot.Bootstrap;

public sealed partial class GameRoot : Node2D
{
    private IGameLoopAdapter _gameLoop = null!;
    private ILocalInputReader _inputReader = null!;
    
    public override void _Ready()
    {
        _gameLoop = DIContainer.Instance.GetService<IGameLoopAdapter>();
    }
    
    /// <summary>
    /// Called every frame to update the ECS world.
    /// This method is responsible for running the ECS update logic.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame.</param>
    public override void _Process(double delta)
    {
        _inputReader.ReadInput();
        
        // Update the ECS world
        _gameLoop.Tick(delta);
    }
	
    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// This method disposes of the ECS world and update runner.
    /// </summary>
    public override void _ExitTree()
    {
        Instance = null!;
        
        _gameLoop.Dispose();
		
        _gameLoop = null!;
		
        base._ExitTree();
    }
}
