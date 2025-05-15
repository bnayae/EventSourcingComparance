using Funds.Abstractions;
using Funds.Events;
using System.Collections.Immutable;


namespace MartenEventSourcing.APIs.Views;

public record BalanceView
{
    public static BalanceView Empty { get; } = new BalanceView
    {
        Id = Guid.Empty,
        Funds = ImmutableDictionary<Currency, double>.Empty,
        IsActive = false,
        Created = false
    };

    public required Guid Id { get; set; }
    public required ImmutableDictionary<Currency, double> Funds { get; set; }
    public bool IsActive { get; set; }
    public bool Created { get; set; }


    public BalanceView Apply(FundsAccountCreated e)
    {
        return new BalanceView
        {
            Id = e.AccountId,
            IsActive = true,
            Funds = ImmutableDictionary<Currency, double>.Empty,
            Created = true,
        };
    }

    public BalanceView Apply(FundsDeposited e)
    {
        if (!Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;
        return this with
        {
            Funds = Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance + e.Data.Amount),
        };
    }

    public BalanceView Apply(FundsWithdrawn e)
    {
        if (!Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;

        //if(oldBalance < e.Data.Amount)
        //    throw new InvalidOperationException($"Not enough funds to withdraw {e.Data.Amount} {e.Data.Currency}");

        return this with
        {
            Funds = Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance - e.Data.Amount),
        };
    }

    public BalanceView Apply(FundsAccountBlocked e)
    {
        return this with
        {
            IsActive = false,
        };
    }

    public BalanceView Apply(FundsAccountUnblocked e)
    {
        return this with
        {
            IsActive = true,
        };
    }

    public BalanceView Apply(FundsAccountClosed e)
    {
        return new BalanceView
        {
            Id = e.AccountId,
            IsActive = false,
            Created = false,
            Funds = ImmutableDictionary<Currency, double>.Empty
        };
    }
}
