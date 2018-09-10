using System.Threading.Tasks;

namespace Bus.EventHandler
{
    public class PaymentFailureEventHandler : IDynamicEventHandler
    {
        public Task Handle(dynamic @event)
        {
            string transactionId = @event.TransactionId;
            // business logic 

            return Task.CompletedTask;
        }
    }
}