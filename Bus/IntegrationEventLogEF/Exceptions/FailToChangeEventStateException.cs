using System;
using System.Collections.Generic;
using System.Text;
using Bus.IntegrationEventLogEF.Models;

namespace Bus.IntegrationEventLogEF.Exceptions
{
    public class FailToChangeEventStateException : Exception
    {
        public IntegrationEventLog IntegrationEventLog { get; }

        public FailToChangeEventStateException(IntegrationEventLog integrationEventLog, Exception innerException) : 
            base($"Fail to mark event:{integrationEventLog.EventTypeName} as {integrationEventLog.State}", innerException)
        {
            IntegrationEventLog = integrationEventLog;
        }
    }
}
