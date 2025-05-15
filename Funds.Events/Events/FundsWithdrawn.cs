using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds withdrawal 
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Data">Common transaction data</param>
[EvDbDefineEventPayload("funds-withdrawn")]
public readonly partial record struct FundsWithdrawn(AccountId AccountId,
                                              FundsTransactionData Data);
