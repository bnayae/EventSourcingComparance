using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds commission taken
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Commission">The commission taken percent (0-1)</param>
[EvDbDefineEventPayload("funds-commission-taken")]
public readonly partial record struct FundsCommissionTaken(AccountId AccountId,
                                              Commission Commission);
