namespace Funds.Abstractions;

public interface IRepository
{
    Task<ulong> AppendEventAsync<T>(Guid accountId, IEnumerable<T> eventsData, CancellationToken cancellationToken = default) where T : IEvDbPayload;
    Task<ulong> AppendEventAsync<T>(Guid accountId, T eventData, CancellationToken cancellationToken = default) where T : IEvDbPayload;
    Task<Balance> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken = default);
}