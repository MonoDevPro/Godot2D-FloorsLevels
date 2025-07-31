using Game.Shared.Scripts.Network.Seralization.Extensions;
using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Data.Input;

/// <summary>
/// Represents an input message sent from the client to the server.
/// </summary>
public struct InputRequest : INetSerializable
{
    public Vector2 Value;
    
    public void Serialize(NetDataWriter writer)
    {
        writer.Serialize(Value);
    }
    
    public void Deserialize(NetDataReader reader)
    {
        Value = reader.DeserializeVector2();
    }
}
