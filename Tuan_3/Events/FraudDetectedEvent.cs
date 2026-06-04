using System;

namespace MasterDBank.Events
{
    // Event được publish khi phát hiện giao dịch khả nghi
    public record FraudDetectedEvent(string AccountId, double Amount, string Reason, DateTime Timestamp) : IEvent
    {
        public FraudDetectedEvent(string accountId, double amount, string reason)
            : this(accountId, amount, reason, DateTime.UtcNow)
        {
        }
    }
}