using Client.Domain.Common;

namespace Client.Domain.ValueObjects.Transforms;

/// <summary>
/// Grid definido por uma posição (top‑left) e um tamanho.
/// </summary>
public readonly record struct Grid(Position Origin, Size Size) : IValueObject
{
    public int Left   => Origin.X;
    public int Top    => Origin.Y;
    public int Right  => Origin.X + Size.Width;
    public int Bottom => Origin.Y + Size.Height;

    /// <summary>Retorna se este retângulo contém a posição dada.</summary>
    public bool Contains(Position p)
        => p.X >= Left && p.X < Right
                       && p.Y >= Top  && p.Y < Bottom;

    /// <summary>Testa interseção com outro retângulo.</summary>
    public bool Intersects(Grid other)
        => Left < other.Right && other.Left < Right
                              && Top  < other.Bottom && other.Top < Bottom;
}
