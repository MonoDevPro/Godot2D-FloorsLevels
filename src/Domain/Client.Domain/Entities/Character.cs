using Client.Domain.Common;
using Client.Domain.Enums;
using Client.Domain.ValueObjects;

namespace Client.Domain.Entities;

public class Character : DomainEntity
{
    public Sprite Sprite { get; private set; }
    public Health Health { get; }
    public Stats Stats   { get; }
    public Movement Movement { get; }
    public Attack Attack { get; }
    
    public Character(
        Sprite sprite,
        Health health,
        Stats stats,
        Movement movement,
        Attack attack)
    {
        Sprite  = sprite;
        Health  = health;
        Stats   = stats;
        Movement    = movement;
        Attack  = attack;
    }
    
    /// <summary>
    /// Altera a vocação do jogador, se for diferente da atual.
    /// </summary>
    public void ChangeVocation(Vocation newVocation)
    {
        if (newVocation == Sprite.Vocation)
            return;
        
        Sprite = Sprite with { Vocation = newVocation };
    }

    /// <summary>
    /// Altera o gênero do jogador, se for diferente do atual.
    /// </summary>
    public void ChangeGender(Gender newGender)
    {
        if (newGender == Sprite.Gender)
            return;

        Sprite = Sprite with { Gender = newGender };
    }
}
