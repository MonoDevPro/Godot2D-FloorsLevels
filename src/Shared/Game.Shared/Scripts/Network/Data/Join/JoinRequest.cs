using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Data.Join;

public struct JoinRequest : INetSerializable
{
    public string Name;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
    }

    public void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
    }
}
