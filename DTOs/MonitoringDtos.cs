using System;
using PatientRecovery.MonitoringService.Models;

namespace PatientRecovery.MonitoringService.DTOs
{
    public class MonitorDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string DeviceId { get; set; }
        public MonitoringStatus Status { get; set; }
        public decimal Temperature { get; set; }
        public int HeartRate { get; set; }
        public int BloodPressureSystolic { get; set; }
        public int BloodPressureDiastolic { get; set; }
        public int RespiratoryRate { get; set; }
        public int OxygenSaturation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AlertDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid MonitorId { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Message { get; set; }
        public bool IsAcknowledged { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
    }
}