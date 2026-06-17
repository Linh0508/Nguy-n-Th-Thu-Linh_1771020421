using System;

namespace MasterDBank.Events
{
    // Event biểu diễn hành động chuyển tiền từ FromId -> ToId.
    // Sử dụng record immutable để dễ serialize và truyền qua event bus.
    public record TransferMoneyEvent(string FromId, string ToId, double Amount, DateTime Timestamp) : IEvent
    {
        public TransferMoneyEvent(string fromId, string toId, double amount)
            : this(fromId, toId, amount, DateTime.UtcNow)
        {
        }
    }
}