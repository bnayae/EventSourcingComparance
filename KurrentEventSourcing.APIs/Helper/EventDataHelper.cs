using EvDb.Core;
using KurrentDB.Client;

namespace KurrentEventSourcing.APIs;

public static class EventDataHelper
{
    public static EventData ToEventData<T>(this T @event)
        where T : IEvDbPayload
    {
        var id = Uuid.NewUuid();
        var data = JsonSerializer.SerializeToUtf8Bytes(@event);
        return new EventData(id, @event.PayloadType, data);
    }

    public static async Task<ulong> ReadCurrentRevision(this KurrentDBClient store, string streamName)
    {
        KurrentDBClient.ReadStreamResult result = store.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start);
        ResolvedEvent last = await result.LastAsync();
        ulong clientTwoRevision = last.Event.EventNumber.ToUInt64();
        return clientTwoRevision;
    }
}
