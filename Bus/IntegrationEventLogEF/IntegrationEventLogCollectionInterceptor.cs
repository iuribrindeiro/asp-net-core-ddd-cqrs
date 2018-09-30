using System.Collections.Generic;
using Bus.IntegrationEventLogEF.Models;
using Castle.Core.Interceptor;

namespace Bus.IntegrationEventLogEF
{
    public class IntegrationEventLogCollectionInterceptor : IInterceptor
    {
        public IList<IntegrationEventLogEntry> IntegrationEventLogEntries { get; set; }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            var value = invocation.ReturnValue;
        }
    }
}
