using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Outbox
{
    public class OutboxMessageConsumer
    {
        public Guid Id { get; set; }
        public string ConsumerType { get; set; } = null!; // Assembly qualified consumer name
        public DateTime ProcessedOn { get; set; }
    }
}
