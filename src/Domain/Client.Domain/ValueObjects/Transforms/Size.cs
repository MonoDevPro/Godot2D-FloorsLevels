using System.Numerics;
using Client.Domain.Common;

namespace Client.Domain.ValueObjects.Transforms
{
    /// <summary>
    /// Representa uma dimensão (largura x altura) em unidades de grid ou pixels.
    /// </summary>
    public readonly record struct Size(int Width, int Height) : IValueObject
    {
        /// <summary>Uma dimensão vazia (0x0).</summary>
        public static Size Zero() => new(0, 0);

        /// <summary>Diz se alguma das dimensões é zero ou negativa.</summary>
        public bool IsEmpty => Width <= 0 || Height <= 0;

        /// <summary>Área (Width * Height).</summary>
        public int Area => Width * Height;

        /// <summary>Redimensiona por um fator escalar.</summary>
        public Size Scale(int factor) 
            => new(Width * factor, Height * factor);

        /// <summary>Converte para Vector2 (pixel ou unidades), útil na Godot.</summary>
        public Vector2 ToVector2() 
            => new(Width, Height);

        public override string ToString() 
            => $"[{Width}×{Height}]";
    }
}
