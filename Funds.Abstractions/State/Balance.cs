// File: Views/Balance.cs
using System.Collections.Immutable;

namespace Funds.Abstractions;

public record Balance
{
    public static Balance Empty { get; } = new Balance
    {
        Id = Guid.Empty,
        Funds = ImmutableDictionary<Currency, double>.Empty,
        IsActive = false,
        Created = false
    };

    public required Guid Id { get; init; }
    public required ImmutableDictionary<Currency, double> Funds { get; init; }
    public bool IsActive { get; init; }
    public bool Created { get; init; }
}