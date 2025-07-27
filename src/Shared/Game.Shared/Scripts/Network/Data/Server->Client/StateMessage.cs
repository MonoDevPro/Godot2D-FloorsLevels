using Arch.Core;
using Game.Shared.Scripts.ECS.Components;
using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Data;

/// <summary>
/// Represents a state message sent from the server to the client.
/// This message contains the state of an entity, including its ID, position, and velocity.
/// It is used for state synchronization in a networked game environment.
/// </summary>
public struct StateMessage : 
    IForEach<NetworkedTag, PositionComponent, VelocityComponent>, 
    INetSerializable
{
    public NetworkedTag NetworkedTag;
    public Vector2 NewPosition;
    public Vector2 NewVelocity;
    
    public void Update(ref NetworkedTag tag, ref PositionComponent position, ref VelocityComponent velocity)
    {
        if (tag.Id != NetworkedTag.Id)
            return;
        
        position.Position = NewPosition;
        velocity.Velocity = NewVelocity;
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetworkedTag.Id);
        writer.Put(NewPosition.X);
        writer.Put(NewPosition.Y);
        writer.Put(NewVelocity.X);
        writer.Put(NewVelocity.Y);
    }

    public void Deserialize(NetDataReader reader)
    {
        NetworkedTag = new NetworkedTag { Id = reader.GetInt() };
        NewPosition = new Vector2(reader.GetFloat(), reader.GetFloat());
        NewVelocity = new Vector2(reader.GetFloat(), reader.GetFloat());
    }
}
