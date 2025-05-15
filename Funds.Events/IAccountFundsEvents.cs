namespace Funds.Events;

[EvDbAttachEventType<FundsAccountCreated>]
[EvDbAttachEventType<FundsDeposited>]
[EvDbAttachEventType<FundsWithdrawn>]
[EvDbAttachEventType<FundsCommissionTaken>]
[EvDbAttachEventType<FundsAccountBlocked>]
[EvDbAttachEventType<FundsAccountUnblocked>]
[EvDbAttachEventType<FundsAccountClosed>]
public partial interface IAccountFundsEvents
{
}
