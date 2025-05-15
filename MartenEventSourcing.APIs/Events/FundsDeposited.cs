using Funds.Abstractions;

namespace Funds.Events;

/// <summary>
/// Funds deposited 
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Data">Common transaction data</param>
public partial record FundsDeposited(AccountId AccountId,
                                              FundsTransactionData Data);
