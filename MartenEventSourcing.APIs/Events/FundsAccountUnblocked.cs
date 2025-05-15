using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account unblocked 
/// </summary>
/// <param name="AccountId">Account identifier</param>
public partial record FundsAccountUnblocked(AccountId AccountId);
