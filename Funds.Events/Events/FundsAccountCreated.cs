using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account created 
/// </summary>
/// <param name="AccountId">Account identifier</param>
[EvDbDefineEventPayload("funds-account-created")]
public readonly partial record struct FundsAccountCreated(AccountId AccountId);
