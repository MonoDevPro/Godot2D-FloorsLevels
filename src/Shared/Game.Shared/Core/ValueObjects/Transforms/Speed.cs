namespace Game.Shared.Core.ValueObjects.Transforms;

/// <summary>
/// Representa a velocidade de movimentação de um personagem em unidades por segundo.
/// </summary>
public readonly record struct Speed
{
    /// <summary>Valor mínimo permitido para Speed (0 = parado).</summary>
    public const float MinValue = 0f;
    /// <summary>Valor máximo razoável para Speed.</summary>
    public const float MaxValue = 100f;

    /// <summary>Valor da velocidade (unidades por segundo).</summary>
    public float Value { get; }

    /// <summary>
    /// Cria uma nova Speed se o valor estiver dentro dos limites, caso contrário lança ArgumentException.
    /// </summary>
    public Speed(float value)
    {
        if (value is < MinValue or > MaxValue)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Velocidade deve estar entre {MinValue} e {MaxValue} unidades/s."
            );

        Value = value;
    }

    /// <summary>
    /// Escala a velocidade por um fator, respeitando os limites.
    /// </summary>
    public Speed Scale(float factor)
        => new(Math.Clamp(Value * factor, MinValue, MaxValue));

    public override string ToString() => $"{Value} units/s";
}
