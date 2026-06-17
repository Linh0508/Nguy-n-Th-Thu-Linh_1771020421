using HaiChanBank.Events;
using MasterDBank.Events;
using System;

namespace HaiChanBank.Services
{
    internal class FraudDetectionService
    {
        private readonly IEventBus _bus;
        private const double FraudThreshold = 1_000_000;

        public FraudDetectionService(IEventBus bus)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _bus.Subscribe<AccountWithdrewEvent>(OnAccountWithdrew);
        }

        private void OnAccountWithdrew(AccountWithdrewEvent @event)
        {
            try
            {
                if (@event == null) return;

                if (@event.Amount > FraudThreshold)
                {
                    Console.WriteLine($"{DateTime.Now} [FraudDetection] Suspicious withdraw detected. Account: {@event.Id}, Amount: {@event.Amount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} [FraudDetection] Error handling withdraw event: {ex.Message}");
            }
        }
    }
}