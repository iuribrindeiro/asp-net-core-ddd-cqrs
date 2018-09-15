using System;
using System.Collections.Generic;
using System.Text;

namespace Bus.EventBus.Exceptions
{
    public class CouldNotConnectToRabbitException : Exception
    {
        public CouldNotConnectToRabbitException() : base("Could not connect to rabbit server")
        {
            
        }
    }
}
