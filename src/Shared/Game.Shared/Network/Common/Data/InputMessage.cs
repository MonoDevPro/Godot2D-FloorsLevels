using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Network.Common.Data;

public struct InputMessage : INetSerializable
{
    public int EntityId; 
    public Vector2 Input;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(EntityId);
        writer.Put(Input.X);
        writer.Put(Input.Y);
    }

    public void Deserialize(NetDataReader reader)
    {
        EntityId = reader.GetInt();
        Input = new Vector2(reader.GetFloat(), reader.GetFloat());
    }
}
