namespace Game.Shared.Scripts.Network.Seralization.Attributes;

/// <summary>
/// Atributo para marcar construtores ou métodos de factory para classes INetSerializable
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
public class TypeConstructorAttribute : Attribute { }
