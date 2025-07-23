using Client.Domain.Common;
using Client.Domain.Enums;

namespace Client.Domain.ValueObjects.States;

/// <summary>
/// Value Object que encapsula o estado do jogo.
/// </summary>
public readonly record struct GameState(GameState.States State = GameState.States.MenuInicial) : IValueObject
{
    public enum States
    {
        MenuInicial,
        Jogando,
        Pausado,
        GameOver,
    }
    
    /// <summary>
    /// Verifica se pode transitar de um estado para outro.
    /// </summary>
    public bool CanTransitionTo(States newState) => (State, newState) switch
    {
        (States.MenuInicial, States.Jogando) => true,
        (States.Jogando,     States.Pausado) => true,
        (States.Pausado,     States.Jogando) => true,
        (States.Jogando,     States.GameOver) => true,
        (States.Pausado,     States.GameOver) => true,
        _ => false
    };

    public override string ToString() => State.ToString();
}
