using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account closed 
/// </summary>
/// <param name="AccountId">Account identifier</param>
public partial record FundsAccountClosed(AccountId Id);
