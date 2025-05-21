using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PatientRecovery.MonitoringService.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace PatientRecoverySystem.MonitoringService.BackgroundServices
{
    public class VitalSignsMonitoringBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VitalSignsMonitoringBackgroundService> _logger;

        public VitalSignsMonitoringBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<VitalSignsMonitoringBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var monitoringService = scope.ServiceProvider.GetRequiredService<IVitalSignsMonitoringService>();
                        await monitoringService.ProcessActiveMonitorsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing vital signs monitors");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}