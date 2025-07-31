using Game.Shared.Scripts.Core.Enums;
using Game.Shared.Scripts.Network.Seralization.Extensions;
using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Entities.Data;

public struct PlayerData : INetSerializable
{
    public PlayerData() { }
    
    public int NetId { get; set; } = 0; // Default Network ID
    public string Name { get; set; } = "Default Character";
    public string Description { get; set; } = "This is a default character description.";
    public VocationEnum Vocation = VocationEnum.None;
    public GenderEnum Gender = GenderEnum.None;
    public float Speed { get; set; } = 200f; // Default movement speed
    public Vector2 Position { get; set; } = Vector2.Zero; // Default position
    public Vector2 Velocity { get; set; } = Vector2.Zero; // Default velocity

    public void UpdateFromResource(ref PlayerData data)
    {
        NetId = data.NetId;
        Name = data.Name;
        Description = data.Description;
        Vocation = data.Vocation;
        Gender = data.Gender;
        Speed = data.Speed;
        Position = data.Position;
        Velocity = data.Velocity;
    }

    public override string ToString()
    {
        return $"PlayerResource(" +
               $"Id: {NetId}, " +
               $"Name: {Name}, " +
               $"Description: {Description}, " +
               $"Vocation: {Vocation}, " +
               $"Gender: {Gender}, " +
               $"Speed: {Speed}), " +
               $"Position: {Position}, " +
               $"Velocity: {Velocity})";
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(NetId);
        writer.Put(Name);
        writer.Put(Description);
        writer.Put((byte)Vocation);
        writer.Put((byte)Gender);
        writer.Put(Speed);
        writer.Serialize(Position);
        writer.Serialize(Velocity);
    }

    public void Deserialize(NetDataReader reader)
    {
        NetId = reader.GetInt();
        Name = reader.GetString();
        Description = reader.GetString();
        Vocation = (VocationEnum)reader.GetByte();
        Gender = (GenderEnum)reader.GetByte();
        Speed = reader.GetFloat();
        Position = reader.DeserializeVector2();
        Velocity = reader.DeserializeVector2();
    }
}
