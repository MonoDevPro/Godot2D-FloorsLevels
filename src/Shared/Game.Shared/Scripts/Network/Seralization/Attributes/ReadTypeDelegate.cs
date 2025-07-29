namespace Game.Shared.Scripts.Network.Seralization.Attributes;

/// <summary>
/// Atributo para marcar métodos estáticos que fornecem delegates de leitura customizados
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ReadTypeDelegateAttribute : Attribute { }
