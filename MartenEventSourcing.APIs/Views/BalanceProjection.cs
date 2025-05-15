using Funds.Events;
using System.Collections.Immutable;

using Marten.Events.Aggregation;
using Funds.Abstractions;

namespace MartenEventSourcing.APIs.Views;

// TODO: check the thread safety of this class

public class BalanceProjection : SingleStreamProjection<BalanceView>
{
    public BalanceView Apply(FundsAccountCreated e, BalanceView view)
    {
        return new BalanceView
        {
            Id = e.Id,
            IsActive = true,
            Funds = ImmutableDictionary<Currency, double>.Empty,
            Created = true,
        };
    }

    public BalanceView Apply(FundsDeposited e, BalanceView view)
    {
        if (!view.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;
        return view with
        {
            Funds = view.Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance + e.Data.Amount),
        };
    }

    public BalanceView Apply(FundsWithdrawn e, BalanceView view)
    {
        if (!view.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;

        //if(oldBalance < e.Data.Amount)
        //    throw new InvalidOperationException($"Not enough funds to withdraw {e.Data.Amount} {e.Data.Currency}");

        return view with
        {
            Funds = view.Funds.Remove(e.Data.Currency)
                     .Add(e.Data.Currency, oldBalance - e.Data.Amount)
        };
    }

    public BalanceView Apply(FundsAccountBlocked e, BalanceView view)
    {
        return view with
        {
            IsActive = false
        };
    }

    public BalanceView Apply(FundsAccountUnblocked e, BalanceView view)
    {
        return view with
        {
            IsActive = true
        };
    }

    public BalanceView Apply(FundsAccountClosed e, BalanceView view)
    {
        return view with
        {
            IsActive = false,
            Created = false,
            Funds = ImmutableDictionary<Currency, double>.Empty
        };
    }
}
