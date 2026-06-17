using HaiChanBank.Events;
using MasterDBank.Events;
using System;
using System.Collections.Generic;

namespace HaiChanBank.Services
{
    public class AccountService
    {
        private readonly Dictionary<string, double> _account = new();
        private readonly IEventBus _bus;

        public AccountService(IEventBus bus)
        {
            _bus = bus;
        }

        public void CreateAccount(string id, string owner)
        {
            _account[id] = 0;
            Console.WriteLine($"{DateTime.Now} Account service - Created account {id} for {owner}");
            _bus.Publish(new AccountCreatedEvent(id, owner));
        }

        public void Deposit(string id, double amount)
        {
            if (!_account.ContainsKey(id))
            {
                Console.WriteLine($"Account {id} does not exist!");
                return;
            }

            _account[id] += amount;
            Console.WriteLine($"{DateTime.Now} Account service - Deposited {amount} for {id}");
            _bus.Publish(new AccountDepositedEvent(id, amount));
        }

        public void Transfer(string fromId, string toId, double amount)
        {
            if (string.Equals(fromId, toId, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Cannot transfer to the same account.");
                return;
            }

            if (!_account.ContainsKey(fromId))
            {
                Console.WriteLine($"Account {fromId} does not exist!");
                return;
            }

            if (!_account.ContainsKey(toId))
            {
                Console.WriteLine($"Account {toId} does not exist!");
                return;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Transfer amount must be positive.");
                return;
            }

            if (_account[fromId] < amount)
            {
                Console.WriteLine($"Account {fromId} has insufficient funds!");
                return;
            }

            _account[fromId] -= amount;
            _account[toId] += amount;

            Console.WriteLine($"{DateTime.Now} Account service - Transferred {amount} from {fromId} to {toId}");

            _bus.Publish(new TransferMoneyEvent(fromId, toId, amount));
            _bus.Publish(new AccountWithdrewEvent(fromId, amount));
            _bus.Publish(new AccountDepositedEvent(toId, amount));
        }
    }
}