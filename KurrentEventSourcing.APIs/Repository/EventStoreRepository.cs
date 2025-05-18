// File: KurrentFunds.API/Repositories/EventStoreRepository.cs
using System.Text;
using System.Text.Json;
using EvDb.Core;
using Funds.Events;
using KurrentDB.Client;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace KurrentEventSourcing.APIs;

public class EventStoreRepository
{
    private readonly KurrentDBClient _eventStoreClient;
    private readonly ILogger<EventStoreRepository> _logger;

    public EventStoreRepository(KurrentDBClient eventStoreClient, ILogger<EventStoreRepository> logger)
    {
        _eventStoreClient = eventStoreClient;
        _logger = logger;
    }

    public async Task<BalanceView> GetBalanceViewAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var streamName = $"account-{accountId}";
        BalanceView balance = BalanceView.Empty;

        try
        {
            var events = _eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                streamName,
                StreamPosition.Start,
                cancellationToken: cancellationToken);

            var readState = await events.ReadState;
            if (readState == ReadState.StreamNotFound)
            {
                return BalanceView.Empty;
            }

            await foreach (var @event in events)
            {
                var eventData = Encoding.UTF8.GetString(@event.Event.Data.Span);
                var eventType = @event.Event.EventType;

                if(eventType == FundsAccountCreated.PAYLOAD_TYPE)
                {
                    balance = balance.Apply(JsonSerializer.Deserialize<FundsAccountCreated>(eventData)!);
                }
                else if (eventType == FundsAccountClosed.PAYLOAD_TYPE)
                {
                    balance = balance.Apply(JsonSerializer.Deserialize<FundsAccountClosed>(eventData)!);
                }
                else if (eventType == FundsAccountBlocked.PAYLOAD_TYPE)
                {
                    balance = balance.Apply(JsonSerializer.Deserialize<FundsAccountBlocked>(eventData)!);
                }
                else if (eventType == FundsAccountUnblocked.PAYLOAD_TYPE)
                {
                    balance = balance.Apply(JsonSerializer.Deserialize<FundsAccountUnblocked>(eventData)!);
                }
                else if (eventType == FundsCommissionTaken.PAYLOAD_TYPE)
                {
                    balance = balance.Apply(JsonSerializer.Deserialize<FundsCommissionTaken>(eventData)!);
                }
                else if (eventType == FundsDeposited.PAYLOAD_TYPE)
                {
                    balance = balance.Apply(JsonSerializer.Deserialize<FundsDeposited>(eventData)!);
                }
                else if (eventType == FundsWithdrawn.PAYLOAD_TYPE)
                {
                    balance = balance.Apply(JsonSerializer.Deserialize<FundsWithdrawn>(eventData)!);
                }
            }

            return balance;
        }
        catch (StreamNotFoundException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving balance for account {AccountId}", accountId);
            throw;
        }
    }

    public async Task<IWriteResult> AppendEventAsync<T>(
        Guid accountId, 
        T eventData,
        CancellationToken cancellationToken = default)
        where T : IEvDbPayload
    {
        var streamName = $"account-{accountId}";
        var data = eventData.ToEventData();

        try
        {
            IWriteResult result = await _eventStoreClient.AppendToStreamAsync(
                streamName,
                StreamState.Any,
                new[] { data },
                cancellationToken: cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error appending event {EventType} to stream {StreamName}", eventData.PayloadType, streamName);
            throw;
        }
    }

    public async Task<IWriteResult> AppendEventAsync<T>(
        Guid accountId, 
        IEnumerable<T> eventsData,
        CancellationToken cancellationToken = default)
        where T : IEvDbPayload
    {
        var streamName = $"account-{accountId}";
        var data = eventsData.Select(eventData => eventData.ToEventData());

        try
        {
            IWriteResult result = await _eventStoreClient.AppendToStreamAsync(
                streamName,
                StreamState.Any,
                data,
                cancellationToken: cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error appending events to stream {StreamName}",  streamName);
            throw;
        }
    }
}