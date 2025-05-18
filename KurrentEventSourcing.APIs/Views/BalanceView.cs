// File: Views/Balance.cs
using Funds.Abstractions;
using Funds.Events;
using System.Collections.Immutable;

namespace KurrentEventSourcing.APIs;

public record BalanceView
{
    public static BalanceView Empty { get; } = new() { Balance = Balance.Empty };


    public required Balance Balance { get; init; }

    public BalanceView Apply(FundsAccountCreated e) =>
        new()
        {
            Balance = new()
            {
                Id = e.AccountId,
                IsActive = true,
                Funds = ImmutableDictionary<Currency, double>.Empty,
                Created = true,
            }
        };

    public BalanceView Apply(FundsDeposited e)
    {
        if (!Balance.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;

        return this with
        {
            Balance = Balance with
            {
                Funds = Balance.Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance + e.Data.Amount),
            }
        };
    }

    public BalanceView Apply(FundsWithdrawn e)
    {
        if (!Balance.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;

        return this with
        {
            Balance = Balance with
            {
                Funds = Balance.Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance - e.Data.Amount),
            }
        };
    }

    public BalanceView Apply(FundsCommissionTaken e)
    {
        if (!Balance.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;

        double commissionValue = e.Data.Amount * e.Commission;
        return this with
        {
            Balance = Balance with
            {
                Funds = Balance.Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance - commissionValue),
            }
        };
    }

    public BalanceView Apply(FundsAccountBlocked e) =>
        this with
        {
            Balance = Balance with
            {
                IsActive = false,
            }
        };

    public BalanceView Apply(FundsAccountUnblocked e) =>
        this with
        {
            Balance = Balance with
            {
                IsActive = true,
            }
        };

    public BalanceView Apply(FundsAccountClosed e) =>
        new()
        {
            Balance = Balance with
            {
                Id = e.AccountId,
                IsActive = false,
                Created = false,
                Funds = ImmutableDictionary<Currency, double>.Empty
            }
        };
}