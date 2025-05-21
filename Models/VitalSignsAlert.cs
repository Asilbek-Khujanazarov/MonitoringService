using System;

namespace PatientRecovery.MonitoringService.Models
{
    public class VitalSignsAlert
    {
        public Guid Id { get; set; }
        public Guid MonitorId { get; set; }
        public Guid PatientId { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; }
        public bool IsAcknowledged { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual VitalSignsMonitor Monitor { get; set; }
    }
}