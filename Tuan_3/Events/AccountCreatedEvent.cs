using HaiChanBank.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDBank.Events
{
    public class AccountCreatedEvent : IEvent
    {
        public string Id { get; set; }
        public string Owner { get; set; }

        public AccountCreatedEvent(string id, string owner)
        {
            Id = id;
            Owner = owner;
        }
    }
}
