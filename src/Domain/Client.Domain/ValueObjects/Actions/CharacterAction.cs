using Client.Domain.Common;

namespace Client.Domain.ValueObjects.Actions;

/// <summary>
/// Value Object que representa uma ação do jogador, validando transições de estado.
/// </summary>
public readonly record struct CharacterAction(CharacterAction.ActionType Type) : IValueObject
{
    public enum ActionType : byte
    {
        Idle,
        Movement,
        Attack,
    }
    
    /// <summary>
    /// Valida transição entre ações.
    /// </summary>
    public bool CanTransitionTo(CharacterAction next) => (Type, next.Type) switch
    {
        (ActionType.Idle,     ActionType.Movement) => true,
        (ActionType.Idle,     ActionType.Attack)   => true,
        (ActionType.Movement, ActionType.Idle)     => true,
        (ActionType.Movement, ActionType.Attack)   => true,
        (ActionType.Attack,   ActionType.Idle)     => true,
        _                                           => false
    };
    
    public override string ToString() => Type.ToString();
    
    /// <summary>
    /// Verifica se a ação é Idle.
    /// </summary>
    public bool IsIdle() => Type == ActionType.Idle;
    
    /// <summary>
    /// Verifica se a ação é de movimento.
    /// </summary>
    public bool IsMovement() => Type == ActionType.Movement;
    
    /// <summary>
    /// Verifica se a ação é de ataque.
    /// </summary>
    public bool IsAttack() => Type == ActionType.Attack;
    
    /// <summary>
    /// Retorna a ação Idle padrão.
    /// </summary>
    public static CharacterAction Idle => new(CharacterAction.ActionType.Idle);

    /// <summary>
    /// Retorna a ação de movimento padrão.
    /// </summary>
    public static CharacterAction Movement => new(CharacterAction.ActionType.Movement);
    /// <summary>
    /// Retorna a ação de ataque padrão.
    /// </summary>
    public static CharacterAction Attack => new(CharacterAction.ActionType.Attack);
    public static CharacterAction Default => Idle;
}
