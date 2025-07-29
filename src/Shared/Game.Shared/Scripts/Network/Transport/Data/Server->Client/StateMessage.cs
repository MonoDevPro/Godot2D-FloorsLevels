using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Transport.Data;

/// <summary>
/// Represents a state message sent from the server to the client.
/// This message contains the state of an entity, including its ID, position, and velocity.
/// It is used for state synchronization in a networked game environment.
/// </summary>
public struct StateMessage : 
    INetSerializable
{
    public int Id;
    public Vector2 NewPosition;
    public Vector2 NewVelocity;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(NewPosition.X);
        writer.Put(NewPosition.Y);
        writer.Put(NewVelocity.X);
        writer.Put(NewVelocity.Y);
    }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetInt();
        NewPosition = new Vector2(reader.GetFloat(), reader.GetFloat());
        NewVelocity = new Vector2(reader.GetFloat(), reader.GetFloat());
    }
}
