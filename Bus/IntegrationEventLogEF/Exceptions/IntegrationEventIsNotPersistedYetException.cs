using System;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Exceptions
{
    public class IntegrationEventIsNotPersistedYetException : Exception
    {
        public IntegrationEventLog IntegrationEventLog { get; set; }

        public IntegrationEventIsNotPersistedYetException(IntegrationEventLog integrationEventLog) : base("The event is not persisted in the storage yet. Please, make sure call a save method before update")
        {
            IntegrationEventLog = integrationEventLog;
        }
    }
}