using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account unblocked 
/// </summary>
/// <param name="AccountId">Account identifier</param>
[EvDbDefineEventPayload("funds-account-unblocked")]
public readonly partial record struct FundsAccountUnblocked(AccountId AccountId);
