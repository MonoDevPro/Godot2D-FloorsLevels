using Client.Domain.ValueObjects;

namespace Client.Domain.Interfaces;

public interface IHealthStore
{
    Health GetHealth();
}