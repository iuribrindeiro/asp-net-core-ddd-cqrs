using System;
using System.Linq;
using Bus.IntegrationEventLogEF.Models;
using Castle.DynamicProxy;

namespace Bus.IntegrationEventLogEF.Storages.Proxy
{
    public class IntegrationEventLogInterceptor : IInterceptor
    {
        private readonly IIntegrationEventLogStorage _inMemoryEventLogStorage;
        private readonly IIntegrationEventLogStorage _mongoEventLogStarage;

        public IntegrationEventLogInterceptor(
            IIntegrationEventLogStorage inMemoryIntegrationEventLogStorage,
            IIntegrationEventLogStorage mongoIntegrationEventLogStorage
        )
        {
            _inMemoryEventLogStorage = inMemoryIntegrationEventLogStorage;
            _mongoEventLogStarage = mongoIntegrationEventLogStorage;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name == "Find")
            {
                var result = (IntegrationEventLog)invocation.ReturnValue;
                if (result == null)
                    invocation.ReturnValue = _mongoEventLogStarage.Find((Guid)invocation.Arguments.First());
            }
            
            invocation.Proceed();
        }
    }
}
