using System.Text.RegularExpressions;

namespace Game.Shared.Scripts.Core.ValueObjects.Names;

/// <summary>
/// Representa o nome de um jogador, aplicando validações de formato e tamanho.
/// </summary>
public readonly record struct Name
{
    private static readonly Regex ValidNamePattern = 
        new(@"^[A-Za-z0-9_]{3,16}$", RegexOptions.Compiled);

    /// <summary>Valor do nome do jogador.</summary>
    public string Value { get; }

    /// <summary>
    /// Cria um novo PlayerName se o valor for válido, caso contrário lança ArgumentException.
    /// </summary>
    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Nome não pode ser vazio ou em branco.", nameof(value));

        if (!ValidNamePattern.IsMatch(value))
            throw new ArgumentException(
                "Nome inválido. Utilize entre 3 e 16 caracteres (A‑Z, 0‑9, underscore).",
                nameof(value)
            );

        Value = value;
    }

    public override string ToString() => Value;
}
