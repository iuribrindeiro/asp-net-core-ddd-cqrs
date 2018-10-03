using System;

namespace Bus.IntegrationEventLogEF.Exceptions
{
    public class IntegrationEventWithIdDoesNotExistsException : Exception
    {
        public Guid EventId { get; }
        
        public IntegrationEventWithIdDoesNotExistsException(Guid eventId) : base($"The event with id: {eventId} does not exists in the storage")
        {
            EventId = eventId;
        }
    }
}