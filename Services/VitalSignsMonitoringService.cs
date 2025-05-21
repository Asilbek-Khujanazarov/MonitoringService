using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientRecoverySystem.MonitoringService.Data;

using PatientRecovery.MonitoringService.Models;
using System.Linq;
using PatientRecoverySystem.MonitoringService.Messaging;

namespace PatientRecovery.MonitoringService.Services
{
    public class VitalSignsMonitoringService : IVitalSignsMonitoringService
    {
        private readonly MonitoringDbContext _context;
        private readonly IRabbitMQService _messageBus;
        private readonly ILogger<VitalSignsMonitoringService> _logger;

        public VitalSignsMonitoringService(
            MonitoringDbContext context,
            IRabbitMQService messageBus,
            ILogger<VitalSignsMonitoringService> logger)
        {
            _context = context;
            _messageBus = messageBus;
            _logger = logger;
        }

        public async Task<VitalSignsMonitor> CreateMonitorAsync(VitalSignsMonitor monitor)
        {
            monitor.CreatedAt = DateTime.UtcNow;
            await _context.VitalSignsMonitors.AddAsync(monitor);
            await _context.SaveChangesAsync();
            return monitor;
        }

        public async Task<VitalSignsMonitor> GetMonitorByIdAsync(Guid id)
        {
            return await _context.VitalSignsMonitors
                .Include(m => m.Alerts)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        public async Task<VitalSignsMonitor> GetMonitorByPatientIdAsync(Guid patientId)
        {
            return await _context.VitalSignsMonitors
                .Include(m => m.Alerts)
                .FirstOrDefaultAsync(m => m.PatientId == patientId && !m.IsDeleted);
        }

        public async Task<IEnumerable<VitalSignsAlert>> GetAlertsByMonitorIdAsync(Guid monitorId)
        {
            return await _context.VitalSignsAlerts
                .Where(a => a.MonitorId == monitorId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<VitalSignsAlert>> GetAlertsByPatientIdAsync(Guid patientId)
        {
            return await _context.VitalSignsAlerts
                .Where(a => a.PatientId == patientId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateMonitorAsync(VitalSignsMonitor monitor)
        {
            monitor.UpdatedAt = DateTime.UtcNow;
            _context.VitalSignsMonitors.Update(monitor);
            await _context.SaveChangesAsync();
        }

        public async Task<VitalSignsAlert> ProcessVitalSignsAsync(VitalSignsMonitor monitor)
        {
            var alert = CheckVitalSigns(monitor);
            if (alert != null)
            {
                alert.CreatedAt = DateTime.UtcNow;
                await _context.VitalSignsAlerts.AddAsync(alert);
                await _context.SaveChangesAsync();

                _messageBus.PublishMessage(
                    System.Text.Json.JsonSerializer.Serialize(alert),
                    "vitalsigns.alert"
                );
            }
            return alert;
        }

        public async Task<bool> UpdateMonitorStatusAsync(Guid monitorId, MonitoringStatus status)
        {
            try
            {
                var monitor = await _context.VitalSignsMonitors.FindAsync(monitorId);
                if (monitor == null)
                    return false;

                monitor.Status = status;
                monitor.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _messageBus.PublishMessage(
                    System.Text.Json.JsonSerializer.Serialize(new { MonitorId = monitorId, Status = status }),
                    "monitor.status.updated"
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating monitor status for monitor {monitorId}");
                return false;
            }
        }

        public async Task AcknowledgeAlertAsync(Guid alertId, string acknowledgedBy)
        {
            var alert = await _context.VitalSignsAlerts.FindAsync(alertId);
            if (alert != null)
            {
                alert.IsAcknowledged = true;
                alert.AcknowledgedBy = acknowledgedBy;
                alert.AcknowledgedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _messageBus.PublishMessage(
                    System.Text.Json.JsonSerializer.Serialize(new { AlertId = alertId, AcknowledgedBy = acknowledgedBy }),
                    "alert.acknowledged"
                );
            }
        }

        public async Task ProcessActiveMonitorsAsync()
        {
            var activeMonitors = await _context.VitalSignsMonitors
                .Where(m => m.Status == MonitoringStatus.Active && !m.IsDeleted)
                .ToListAsync();

            foreach (var monitor in activeMonitors)
            {
                try
                {
                    var alert = await ProcessVitalSignsAsync(monitor);
                    if (alert != null)
                    {
                        _logger.LogInformation($"Alert generated for monitor {monitor.Id}: {alert.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing monitor {monitor.Id} for patient {monitor.PatientId}");
                }
            }
        }

        private VitalSignsAlert CheckVitalSigns(VitalSignsMonitor monitor)
        {
            if (monitor.Temperature > 38.5m)
            {
                return CreateAlert(monitor, AlertType.HighTemperature, AlertSeverity.High);
            }
            if (monitor.Temperature < 35.0m)
            {
                return CreateAlert(monitor, AlertType.LowTemperature, AlertSeverity.High);
            }
            if (monitor.HeartRate > 100)
            {
                return CreateAlert(monitor, AlertType.HighHeartRate, AlertSeverity.Medium);
            }
            if (monitor.HeartRate < 60)
            {
                return CreateAlert(monitor, AlertType.LowHeartRate, AlertSeverity.High);
            }
            if (monitor.OxygenSaturation < 95)
            {
                return CreateAlert(monitor, AlertType.LowOxygenSaturation, AlertSeverity.Critical);
            }

            return null;
        }

        private VitalSignsAlert CreateAlert(VitalSignsMonitor monitor, AlertType type, AlertSeverity severity)
        {
            return new VitalSignsAlert
            {
                Id = Guid.NewGuid(),
                MonitorId = monitor.Id,
                PatientId = monitor.PatientId,
                Type = type,
                Severity = severity,
                Message = $"Alert: {type} detected for patient {monitor.PatientId}",
                IsAcknowledged = false,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}