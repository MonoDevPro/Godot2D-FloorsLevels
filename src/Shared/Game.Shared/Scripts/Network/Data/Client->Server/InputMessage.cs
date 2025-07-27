using Arch.Core;
using Game.Shared.Scripts.ECS.Components;
using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Data;

/// <summary>
/// Represents an input message sent from the client to the server.
/// </summary>
public struct InputMessage : 
    IForEach<NetworkedTag, InputCommandComponent>, 
    INetSerializable
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

    public void Update(ref NetworkedTag t0Component, ref InputCommandComponent t1Component)
    {
        if (t0Component.Id != EntityId)
            return;

        t1Component.Input = Input; // Update the input command component with the received input
    }
}
