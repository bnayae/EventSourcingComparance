using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account blocked 
/// </summary>
/// <param name="AccountId">Account identifier</param>
[EvDbDefineEventPayload("funds-account-blocked")]
public readonly partial record struct FundsAccountBlocked(AccountId AccountId);
