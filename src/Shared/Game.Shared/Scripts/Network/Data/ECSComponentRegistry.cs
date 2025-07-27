using Game.Shared.Scripts.ECS.Components;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Data;

public static class ECSComponentRegistry
{
    /// <summary>
    /// Registra todos os components do ECS no NetPacketProcessor fornecido
    /// </summary>
    /// <param name="packetProcessor">Processador de pacotes para registrar os tipos</param>
    public static void RegisterECSComponents(NetPacketProcessor packetProcessor)
    {
        ArgumentNullException.ThrowIfNull(packetProcessor);

        packetProcessor.RegisterNestedType<StateMessage>();
        
        packetProcessor.RegisterNestedType<NetworkedTag>(
            (writer, component) => { writer.Put(component.Id); },
            reader => new NetworkedTag { Id = reader.GetInt() });
    }
}


/*
/// <summary>
/// Centralizador responsável por registrar todos os components do ECS
/// para serialização de rede usando LiteNetLib
/// </summary>
public static class ECSComponentRegistry
{
    /// <summary>
    /// Registra todos os components do ECS no NetPacketProcessor fornecido
    /// </summary>
    /// <param name="packetProcessor">Processador de pacotes para registrar os tipos</param>
    public static void RegisterAllECSComponents(NetPacketProcessor packetProcessor)
    {
        ArgumentNullException.ThrowIfNull(packetProcessor);

        RegisterCoreComponents(packetProcessor);
        RegisterMovementComponents(packetProcessor);
        RegisterTagComponents(packetProcessor);
        RegisterAIComponents(packetProcessor);
    }

    /// <summary>
    /// Registra apenas os components mais comuns usados em rede
    /// </summary>
    public static void RegisterNetworkComponents(NetPacketProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(processor);

        // InputCommandComponent
        processor.RegisterNestedType<InputCommandComponent>(
            (writer, component) => { writer.Put(component.Input.X); writer.Put(component.Input.Y); },
            reader => new InputCommandComponent { Input = new Vector2(reader.GetFloat(), reader.GetFloat()) });

        // PositionComponent
        processor.RegisterNestedType<PositionComponent>(
            (writer, component) => { writer.Put(component.Position.X); writer.Put(component.Position.Y); },
            reader => new PositionComponent { Position = new Vector2(reader.GetFloat(), reader.GetFloat()) });

        // VelocityComponent
        processor.RegisterNestedType<VelocityComponent>(
            (writer, component) => { writer.Put(component.Velocity.X); writer.Put(component.Velocity.Y); },
            reader => new VelocityComponent { Velocity = new Vector2(reader.GetFloat(), reader.GetFloat()) });

        // SpeedComponent
        processor.RegisterNestedType<SpeedComponent>(
            (writer, component) => writer.Put(component.Speed),
            reader => new SpeedComponent { Speed = reader.GetFloat() });
    }

    /// <summary>
    /// Registra components principais do sistema
    /// </summary>
    private static void RegisterCoreComponents(NetPacketProcessor packetProcessor)
    {
        // InputCommandComponent
        packetProcessor.RegisterNestedType<InputCommandComponent>(
            (writer, component) =>
            {
                writer.Put(component.Input.X);
                writer.Put(component.Input.Y);
            },
            reader => new InputCommandComponent
            {
                Input = new Vector2(reader.GetFloat(), reader.GetFloat())
            });

        // PositionComponent
        packetProcessor.RegisterNestedType<PositionComponent>(
            (writer, component) =>
            {
                writer.Put(component.Position.X);
                writer.Put(component.Position.Y);
            },
            reader => new PositionComponent
            {
                Position = new Vector2(reader.GetFloat(), reader.GetFloat())
            });

        // SceneRefComponent - Note: Node references cannot be serialized over network
        // This is a client-only component and should not be sent over network
        // If needed, you could serialize an ID to reconstruct the reference on the other side
    }

    /// <summary>
    /// Registra components relacionados a movimento
    /// </summary>
    private static void RegisterMovementComponents(NetPacketProcessor packetProcessor)
    {
        // VelocityComponent
        packetProcessor.RegisterNestedType<VelocityComponent>(
            (writer, component) =>
            {
                writer.Put(component.Velocity.X);
                writer.Put(component.Velocity.Y);
            },
            reader => new VelocityComponent
            {
                Velocity = new Vector2(reader.GetFloat(), reader.GetFloat())
            });

        // SpeedComponent
        packetProcessor.RegisterNestedType<SpeedComponent>(
            (writer, component) => writer.Put(component.Speed),
            reader => new SpeedComponent { Speed = reader.GetFloat() });
    }

    /// <summary>
    /// Registra components que são tags (sem dados ou com dados mínimos)
    /// </summary>
    private static void RegisterTagComponents(NetPacketProcessor packetProcessor)
    {
        // LocalTag - Component vazio, apenas marker
        packetProcessor.RegisterNestedType<LocalTag>(
            (writer, component) => { /* Tag vazia - nada para serializar #1# },
            reader => new LocalTag());

        // Note: Tags são geralmente usadas apenas localmente
        // e raramente precisam ser serializadas pela rede
    }

    /// <summary>
    /// Registra components relacionados a IA e comportamento
    /// </summary>
    private static void RegisterAIComponents(NetPacketProcessor packetProcessor)
    {
        // NPCPatrolTag
        packetProcessor.RegisterNestedType<NPCPatrolTag>(
            (writer, component) =>
            {
                // Serializa array de patrol points
                writer.Put(component.PatrolPoints?.Length ?? 0);
                if (component.PatrolPoints != null)
                {
                    foreach (var point in component.PatrolPoints)
                    {
                        writer.Put(point.X);
                        writer.Put(point.Y);
                    }
                }
                writer.Put(component.CurrentIndex);
            },
            reader =>
            {
                var length = reader.GetInt();
                Vector2[] patrolPoints = null;

                if (length > 0)
                {
                    patrolPoints = new Vector2[length];
                    for (int i = 0; i < length; i++)
                    {
                        patrolPoints[i] = new Vector2(reader.GetFloat(), reader.GetFloat());
                    }
                }

                return new NPCPatrolTag
                {
                    PatrolPoints = patrolPoints,
                    CurrentIndex = reader.GetInt()
                };
            });
    }

    /// <summary>
    /// Método helper para registrar apenas components que devem ser sincronizados pela rede
    /// Exclui components que são client-only ou server-only
    /// </summary>
    public static void RegisterNetworkSyncComponents(NetPacketProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(processor);

        // Apenas components que fazem sentido sincronizar
        RegisterCoreComponents(processor);
        RegisterMovementComponents(processor);
        RegisterAIComponents(processor);

        // Note: LocalTag e SceneRefComponent são excluídos propositalmente
        // pois são específicos de cada lado (client/server)
    }
}
*/
