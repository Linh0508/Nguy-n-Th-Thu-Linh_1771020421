using System;

namespace MasterDBank.Events
{
    // Event biểu diễn hành vi rút tiền của 1 account
    // Giữ consistent với các event khác (record, namespace MasterDBank.Events)
    public record AccountWithdrewEvent(string Id, double Amount) : IEvent;
}