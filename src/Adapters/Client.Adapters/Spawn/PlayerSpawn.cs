using Client.Infrastructure.ECS.Entities;
using Godot;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Adapters.Spawn;

public partial class PlayerSpawn : Node
{
    private readonly Dictionary<Guid, Tuple<BaseBody2D, int>> _players = new Dictionary<Guid, Tuple<BaseBody2D, int>>();
    public void Spawn()
    {
    }
}