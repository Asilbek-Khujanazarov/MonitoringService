using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatientRecovery.MonitoringService.Models;

namespace PatientRecovery.MonitoringService.Services
{
    public interface IVitalSignsMonitoringService
    {
        Task<VitalSignsMonitor> CreateMonitorAsync(VitalSignsMonitor monitor);
        Task<VitalSignsMonitor> GetMonitorByIdAsync(Guid id);
        Task<VitalSignsMonitor> GetMonitorByPatientIdAsync(Guid patientId);
        Task<IEnumerable<VitalSignsAlert>> GetAlertsByMonitorIdAsync(Guid monitorId);
        Task<IEnumerable<VitalSignsAlert>> GetAlertsByPatientIdAsync(Guid patientId);
        Task<VitalSignsAlert> ProcessVitalSignsAsync(VitalSignsMonitor vitalSigns);
        Task<bool> UpdateMonitorStatusAsync(Guid monitorId, MonitoringStatus status);
        Task AcknowledgeAlertAsync(Guid alertId, string acknowledgedBy);
        Task UpdateMonitorAsync(VitalSignsMonitor monitor);
        Task ProcessActiveMonitorsAsync();
    }
}