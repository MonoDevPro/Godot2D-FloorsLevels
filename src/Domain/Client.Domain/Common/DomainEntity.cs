using System;

namespace Client.Domain.Common;

public abstract class DomainEntity
{
    public Guid Id { get; } = Guid.NewGuid();
}