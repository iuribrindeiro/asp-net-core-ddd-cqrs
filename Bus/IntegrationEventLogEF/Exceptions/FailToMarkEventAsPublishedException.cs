using System;
using System.Collections.Generic;
using System.Text;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Exceptions
{
    public class FailToMarkEventAsPublishedException : Exception
    {
        public IntegrationEventLogEntry IntegrationEventLogEntry { get; }

        public FailToMarkEventAsPublishedException(IntegrationEventLogEntry integrationEventLog, Exception innerException) : 
            base($"Fail to mark event:{integrationEventLog.EventTypeName} as published", innerException)
        {
            IntegrationEventLogEntry = integrationEventLog;
        }
    }
}
