using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Bus.EventBus.RabbitAggregate
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        void Connect();

        IModel CreateModel();
    }
}
