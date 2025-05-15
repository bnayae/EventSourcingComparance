using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds commission taken
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Commission">The commission taken percent (0-1)</param>
public partial record FundsCommissionTaken(AccountId Id,
                                              double Commission);
