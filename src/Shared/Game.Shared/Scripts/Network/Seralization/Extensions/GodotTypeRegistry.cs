using Godot;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Seralization.Extensions;

/// <summary>
/// Centralizador responsável por registrar todos os tipos específicos do Godot
/// para serialização de rede usando LiteNetLib
/// </summary>
public static class GodotTypeRegistry
{
    /// <summary>
    /// Registra todos os tipos do Godot no NetPacketProcessor fornecido
    /// </summary>
    /// <param name="packetProcessor">Processador de pacotes para registrar os tipos</param>
    public static void RegisterAllGodotTypes(NetPacketProcessor packetProcessor)
    {
        ArgumentNullException.ThrowIfNull(packetProcessor);

        RegisterVectorTypes(packetProcessor);
        RegisterColorTypes(packetProcessor);
        RegisterRectTypes(packetProcessor);
        RegisterTransformTypes(packetProcessor);
        RegisterQuaternionTypes(packetProcessor);
        RegisterPlaneTypes(packetProcessor);
    }
    
    /// <summary>
    /// Register only the most common types: Vector2, Vector3, Color.
    /// </summary>
    public static void RegisterBasicGodotTypes(NetPacketProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(processor);

        // Vector2
        processor.RegisterNestedType<Vector2>(
            (writer, v) => { writer.Put(v.X); writer.Put(v.Y); },
            reader => new Vector2(reader.GetFloat(), reader.GetFloat()));

        // Vector3
        processor.RegisterNestedType<Vector3>(
            (writer, v) => { writer.Put(v.X); writer.Put(v.Y); writer.Put(v.Z); },
            reader => new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat()));

        // Color
        processor.RegisterNestedType<Color>(
            (writer, v) => { writer.Put(v.R); writer.Put(v.G); writer.Put(v.B); writer.Put(v.A); },
            reader => new Color(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat()));
    }

    /// <summary>
    /// Registra tipos de vetores (Vector2, Vector3, etc.)
    /// </summary>
    private static void RegisterVectorTypes(NetPacketProcessor packetProcessor)
    {
        // Vector2
        packetProcessor.RegisterNestedType<Vector2>(
            (writer, value) => { writer.Put(value.X); writer.Put(value.Y); },
            reader => new Vector2(reader.GetFloat(), reader.GetFloat()));

        // Vector2I
        packetProcessor.RegisterNestedType<Vector2I>(
            (writer, value) => { writer.Put(value.X); writer.Put(value.Y); },
            reader => new Vector2I(reader.GetInt(), reader.GetInt()));

        // Vector3
        packetProcessor.RegisterNestedType<Vector3>(
            (writer, value) => { writer.Put(value.X); writer.Put(value.Y); writer.Put(value.Z); },
            reader => new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat()));

        // Vector3I
        packetProcessor.RegisterNestedType<Vector3I>(
            (writer, value) => { writer.Put(value.X); writer.Put(value.Y); writer.Put(value.Z); },
            reader => new Vector3I(reader.GetInt(), reader.GetInt(), reader.GetInt()));

        // Vector4
        packetProcessor.RegisterNestedType<Vector4>(
            (writer, value) => { writer.Put(value.X); writer.Put(value.Y); writer.Put(value.Z); writer.Put(value.W); },
            reader => new Vector4(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat()));

        // Vector4I
        packetProcessor.RegisterNestedType<Vector4I>(
            (writer, value) => { writer.Put(value.X); writer.Put(value.Y); writer.Put(value.Z); writer.Put(value.W); },
            reader => new Vector4I(reader.GetInt(), reader.GetInt(), reader.GetInt(), reader.GetInt()));
    }

    /// <summary>
    /// Registra tipos de cores
    /// </summary>
    private static void RegisterColorTypes(NetPacketProcessor packetProcessor)
    {
        // Color
        packetProcessor.RegisterNestedType<Color>(
            (writer, value) => { writer.Put(value.R); writer.Put(value.G); writer.Put(value.B); writer.Put(value.A); },
            reader => new Color(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat()));
    }

    /// <summary>
    /// Registra tipos de retângulos
    /// </summary>
    private static void RegisterRectTypes(NetPacketProcessor packetProcessor)
    {
        // Rect2
        packetProcessor.RegisterNestedType<Rect2>(
            (writer, value) => 
            { 
                writer.Put(value.Position.X); writer.Put(value.Position.Y);
                writer.Put(value.Size.X); writer.Put(value.Size.Y);
            },
            reader => new Rect2(
                reader.GetFloat(), reader.GetFloat(),
                reader.GetFloat(), reader.GetFloat()));

        // Rect2I
        packetProcessor.RegisterNestedType<Rect2I>(
            (writer, value) => 
            { 
                writer.Put(value.Position.X); writer.Put(value.Position.Y);
                writer.Put(value.Size.X); writer.Put(value.Size.Y);
            },
            reader => new Rect2I(
                reader.GetInt(), reader.GetInt(),
                reader.GetInt(), reader.GetInt()));
    }

    /// <summary>
    /// Registra tipos de transformações
    /// </summary>
    private static void RegisterTransformTypes(NetPacketProcessor packetProcessor)
    {
        // Transform2D
        packetProcessor.RegisterNestedType<Transform2D>(
            (writer, value) => 
            { 
                // Serializa como 6 floats (matrix 2x3)
                writer.Put(value.X.X); writer.Put(value.X.Y);
                writer.Put(value.Y.X); writer.Put(value.Y.Y);
                writer.Put(value.Origin.X); writer.Put(value.Origin.Y);
            },
            reader => new Transform2D(
                new Vector2(reader.GetFloat(), reader.GetFloat()),
                new Vector2(reader.GetFloat(), reader.GetFloat()),
                new Vector2(reader.GetFloat(), reader.GetFloat())));

        // Transform3D
        packetProcessor.RegisterNestedType<Transform3D>(
            (writer, value) => 
            { 
                // Serializa basis (9 floats) + origin (3 floats)
                var basis = value.Basis;
                writer.Put(basis.X.X); writer.Put(basis.X.Y); writer.Put(basis.X.Z);
                writer.Put(basis.Y.X); writer.Put(basis.Y.Y); writer.Put(basis.Y.Z);
                writer.Put(basis.Z.X); writer.Put(basis.Z.Y); writer.Put(basis.Z.Z);
                writer.Put(value.Origin.X); writer.Put(value.Origin.Y); writer.Put(value.Origin.Z);
            },
            reader => new Transform3D(
                new Basis(
                    new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat()),
                    new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat()),
                    new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat())),
                new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat())));
    }

    /// <summary>
    /// Registra tipos de quaternion
    /// </summary>
    private static void RegisterQuaternionTypes(NetPacketProcessor packetProcessor)
    {
        // Quaternion
        packetProcessor.RegisterNestedType<Quaternion>(
            (writer, value) => { writer.Put(value.X); writer.Put(value.Y); writer.Put(value.Z); writer.Put(value.W); },
            reader => new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat()));
    }

    /// <summary>
    /// Registra tipos de planos
    /// </summary>
    private static void RegisterPlaneTypes(NetPacketProcessor packetProcessor)
    {
        // Plane
        packetProcessor.RegisterNestedType<Plane>(
            (writer, value) => { writer.Put(value.Normal.X); writer.Put(value.Normal.Y); writer.Put(value.Normal.Z); writer.Put(value.D); },
            reader => new Plane(new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat()), reader.GetFloat()));
    }
}
