namespace Game.Shared.Scripts.Network.Seralization.Attributes;

/// <summary>
/// Atributo para marcar métodos estáticos que fornecem delegates de escrita/leitura customizados
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class WriteTypeDelegateAttribute : Attribute { }
