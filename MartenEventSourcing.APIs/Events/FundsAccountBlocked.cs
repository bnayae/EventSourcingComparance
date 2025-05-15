
using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account blocked 
/// </summary>
/// <param name="AccountId">Account identifier</param>
public partial record FundsAccountBlocked(AccountId AccountId);
