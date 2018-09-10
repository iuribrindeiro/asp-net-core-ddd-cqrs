using Bus.Events.Base;

namespace Bus.Events
{
    public class PaymentFailureEvent : Event
    {
        public string TransactionId { get; }
        public string Description { get; }

        public PaymentFailureEvent(string transactionId, string description)
        {
            TransactionId = transactionId;
            Description = description;
        }
    }
}