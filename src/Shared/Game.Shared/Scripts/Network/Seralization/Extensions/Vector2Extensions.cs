using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Seralization.Extensions;

public static class Vector2Extensions
{
    /// <summary>
    /// Serializes a Vector2 to a NetDataWriter.
    /// </summary>
    public static void Serialize(this NetDataWriter writer, Vector2 vector)
    {
        writer.Put(vector.X);
        writer.Put(vector.Y);
    }

    /// <summary>
    /// Deserializes a Vector2 from a NetDataReader.
    /// </summary>
    public static Vector2 DeserializeVector2(this NetDataReader reader)
    {
        return new Vector2(reader.GetFloat(), reader.GetFloat());
    }
}
