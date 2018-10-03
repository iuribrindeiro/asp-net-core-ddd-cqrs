namespace Bus.IntegrationEventLogEF.Storages.Proxy
{
    public interface IIntegrationEventLogStorageFactory
    {
        IIntegrationEventLogStorage GetStorage();
    }
}