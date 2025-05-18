using Funds.Abstractions;
using Funds.Events;
using Marten.Events.Aggregation;
using System.Collections.Immutable;

namespace MartenEventSourcing.APIs.Views;

// TODO: check the thread safety of this class

public class BalanceProjection : SingleStreamProjection<Balance>
{
    public Balance Apply(FundsAccountCreated e, Balance view)
    {
        return new Balance
        {
            Id = e.AccountId,
            IsActive = true,
            Funds = ImmutableDictionary<Currency, double>.Empty,
            Created = true,
        };
    }

    public Balance Apply(FundsDeposited e, Balance view)
    {
        if (!view.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;
        return view with
        {
            Funds = view.Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance + e.Data.Amount),
        };
    }

    public Balance Apply(FundsWithdrawn e, Balance view)
    {
        if (!view.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;

        return view with
        {
            Funds = view.Funds.Remove(e.Data.Currency)
                     .Add(e.Data.Currency, oldBalance - e.Data.Amount)
        };
    }

    public Balance Apply(FundsCommissionTaken e, Balance view)
    {
        if (!view.Funds.TryGetValue(e.Data.Currency, out var oldBalance))
            oldBalance = 0;

        double commissionValue = e.Data.Amount * e.Commission;
        return view with
        {
            Funds = view.Funds.Remove(e.Data.Currency)
                         .Add(e.Data.Currency, oldBalance - commissionValue)
        };
    }

    public Balance Apply(FundsAccountBlocked e, Balance view)
    {
        return view with
        {
            IsActive = false
        };
    }

    public Balance Apply(FundsAccountUnblocked e, Balance view)
    {
        return view with
        {
            IsActive = true
        };
    }

    public Balance Apply(FundsAccountClosed e, Balance view)
    {
        return view with
        {
            IsActive = false,
            Created = false,
            Funds = ImmutableDictionary<Currency, double>.Empty
        };
    }
}
