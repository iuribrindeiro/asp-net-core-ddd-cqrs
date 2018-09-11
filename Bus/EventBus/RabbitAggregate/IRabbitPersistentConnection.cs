using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Bus.EventBus.RabbitAggregate
{
    public interface IRabbitPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
