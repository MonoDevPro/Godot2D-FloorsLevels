using Game.Shared.Scripts.Network.Seralization.Extensions;
using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Data.Sync;

/// <summary>
/// Represents a state message sent from the server to the client.
/// This message contains the state of an entity, including its ID, position, and velocity.
/// It is used for state synchronization in a networked game environment.
/// </summary>
public struct PositionSyncMessage : 
    INetSerializable
{
    public int NetId { get; set; }
    public Vector2 Position;
    public Vector2 Velocity;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetId);
        writer.Serialize(Position);
        writer.Serialize(Velocity);
    }

    public void Deserialize(NetDataReader reader)
    {
        NetId = reader.GetInt();
        Position = reader.DeserializeVector2();
        Velocity = reader.DeserializeVector2();
    }
}
