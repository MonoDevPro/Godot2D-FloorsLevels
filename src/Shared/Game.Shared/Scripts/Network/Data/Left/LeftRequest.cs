using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Data.Left;

public struct LeftRequest : INetSerializable
{
    public void Serialize(NetDataWriter writer)
    {
        // No data to serialize for left message
    }

    public void Deserialize(NetDataReader reader)
    {
        // No data to deserialize for left message
    }
}
