namespace Game.Shared.Scripts.Network.Seralization.Attributes;

/// <summary>
/// Atributo para marcar tipos que devem ser automaticamente registrados no NetPacketProcessor
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class AutoRegisterTypeAttribute : Attribute
{
    /// <summary>
    /// Se verdadeiro, força o uso de delegates customizados (se disponíveis)
    /// </summary>
    public bool UseCustomDelegates { get; set; } = false;
}
