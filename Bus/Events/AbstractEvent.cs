using System;

namespace Bus.Events
{
    public abstract class AbstractEvent
    {
        protected AbstractEvent()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime CreatedAt { get; }
    }
}