using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDBank.Events
{
    public record AccountDepositedEvent(string Id, double Amount) : IEvent;
}
