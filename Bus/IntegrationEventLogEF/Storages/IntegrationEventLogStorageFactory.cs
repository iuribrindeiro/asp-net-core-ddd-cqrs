using Bus.IntegrationEventLogEF.Storages.Proxy;
using Castle.DynamicProxy;

namespace Bus.IntegrationEventLogEF.Storages
{
    public class IntegrationEventLogStorageFactory : IIntegrationEventLogStorageFactory
    {
        private readonly IIntegrationEventLogStorage _inMemoryIntegrationEventLogStorage;
        private readonly IIntegrationEventLogStorage _mongoIntegrationEventLogStorage;
        private readonly ProxyGenerator _proxyGenerator;

        public IntegrationEventLogStorageFactory(IIntegrationEventLogStorage inMemoryIntegrationEventLogStorage, IIntegrationEventLogStorage mongoIntegrationEventLogStorage)
        {
            _inMemoryIntegrationEventLogStorage = inMemoryIntegrationEventLogStorage;
            _mongoIntegrationEventLogStorage = mongoIntegrationEventLogStorage;
            _proxyGenerator = new ProxyGenerator();
        }

        public IIntegrationEventLogStorage GetStorage()
        {
            var interceptor = new IntegrationEventLogInterceptor(_inMemoryIntegrationEventLogStorage,
                _mongoIntegrationEventLogStorage);
            return _proxyGenerator.CreateInterfaceProxyWithoutTarget<IIntegrationEventLogStorage>(interceptor);
        }
    }
}