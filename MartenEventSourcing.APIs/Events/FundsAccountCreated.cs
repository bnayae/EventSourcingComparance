using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds account created 
/// </summary>
/// <param name="Id">Account identifier</param>
public partial record FundsAccountCreated(AccountId Id);
