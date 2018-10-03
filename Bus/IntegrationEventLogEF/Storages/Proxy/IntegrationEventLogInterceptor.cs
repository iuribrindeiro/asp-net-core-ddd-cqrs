using Bus.IntegrationEventLogEF.Models;
using Castle.Core.Interceptor;

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
            ChangeInterceptorTargetToInMemory(invocation);
            
            if (invocation.Method.Name == "Find")
            {
                var result = (IntegrationEventLog)invocation.ReturnValue;
                if (result == null)
                    ChangeInterceptorTargetToMongo(invocation);
            }
            
            invocation.Proceed();
        }

        private void ChangeInterceptorTargetToInMemory(IInvocation invocation)
        {
            var changeProxyTarget = invocation as IChangeProxyTarget;
            changeProxyTarget.ChangeInvocationTarget(_inMemoryEventLogStorage); 
        }

        private void ChangeInterceptorTargetToMongo(IInvocation invocation)
        {
            var changeProxyTarget = invocation as IChangeProxyTarget;
            changeProxyTarget.ChangeInvocationTarget(_mongoEventLogStarage);
        }
    }
}
