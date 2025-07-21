using Godot;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS;

namespace Client.Adapters.Bootstrap;

public sealed partial class GameRoot : Node2D
{
	public WorldECS WorldECS { get; private set; } = DIContainer.Instance.GetService<WorldECS>();
	public UpdateECS UpdateECS { get; private set; } = DIContainer.Instance.GetService<UpdateECS>();
	
	public override void _Ready() { }
	
	/// <summary>
	/// Called every frame to update the ECS world.
	/// This method is responsible for running the ECS update logic.
	/// </summary>
	/// <param name="delta">The time elapsed since the last frame.</param>
	public override void _Process(double delta)
	{
		// Update the ECS world
		UpdateECS.Update((float)delta);
	}
	
	/// <summary>
	/// Called when the node is removed from the scene tree.
	/// This method disposes of the ECS world and update runner.
	/// </summary>
	public override void _ExitTree()
	{
		WorldECS.Dispose();
		UpdateECS.Dispose();
		
		WorldECS = null!;
		UpdateECS = null!;
		
		base._ExitTree();
	}

}