using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Network.Common.Data;

public struct StateMessage : INetSerializable
{
    public int EntityId; 
    public Vector2 Position;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(EntityId);
        writer.Put(Position.X);
        writer.Put(Position.Y);
    }

    public void Deserialize(NetDataReader reader)
    {
        EntityId = reader.GetInt();
        Position = new Vector2(reader.GetFloat(), reader.GetFloat());
    }
}
