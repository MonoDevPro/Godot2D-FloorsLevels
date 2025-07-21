using Client.Domain.ValueObjects;

namespace Client.Domain.Interfaces;

public interface IStatsStore
{
    Stats GetStats();
}