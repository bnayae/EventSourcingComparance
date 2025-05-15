using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account closed 
/// </summary>
/// <param name="AccountId">Account identifier</param>
[EvDbDefineEventPayload("funds-account-closed")]
public readonly partial record struct FundsAccountClosed(AccountId AccountId);
