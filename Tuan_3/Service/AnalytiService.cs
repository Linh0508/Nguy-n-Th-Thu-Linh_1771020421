using HaiChanBank.Events;
using MasterDBank.Events;
using System;

namespace HaiChanBank.Services
{
    internal class AnalyticService
    {
        private double _totalDeposits = 0;
        private double _totalWithdraws = 0;
        private int _totalAccounts = 0;

        public void LogInfo()
        {
            Console.WriteLine($"{DateTime.Now} [Analytic Service]");
            Console.WriteLine($"Total Accounts: {_totalAccounts}");
            Console.WriteLine($"Total Deposits: {_totalDeposits}");
            Console.WriteLine($"Total Withdraws: {_totalWithdraws}");
        }

        public AnalyticService(IEventBus bus)
        {
            bus.Subscribe<AccountCreatedEvent>(OnAccountCreated);
            bus.Subscribe<AccountDepositedEvent>(OnAccountDeposited);
            bus.Subscribe<AccountWithdrewEvent>(OnAccountWithdrew);
        }

        private void OnAccountCreated(AccountCreatedEvent @event)
        {
            _totalAccounts++;
            LogInfo();
        }

        private void OnAccountDeposited(AccountDepositedEvent @event)
        {
            _totalDeposits += @event.Amount;
            LogInfo();
        }

        private void OnAccountWithdrew(AccountWithdrewEvent @event)
        {
            _totalWithdraws += @event.Amount;
            LogInfo();
        }
    }
}