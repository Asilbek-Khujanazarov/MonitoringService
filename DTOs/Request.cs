using System;
using PatientRecovery.MonitoringService.Models;

namespace PatientRecovery.MonitoringService.DTOs
{
    public class CreateMonitorRequest
    {
        public Guid PatientId { get; set; }
        public string DeviceId { get; set; }
    }

    public class UpdateMonitorStatusRequest
    {
        public MonitoringStatus Status { get; set; }
        public string Message { get; set; }
    }

    public class UpdateVitalSignsRequest
    {
        public decimal Temperature { get; set; }
        public int HeartRate { get; set; }
        public int BloodPressureSystolic { get; set; }
        public int BloodPressureDiastolic { get; set; }
        public int RespiratoryRate { get; set; }
        public int OxygenSaturation { get; set; }
    }

    public class AcknowledgeAlertRequest
    {
        public string AcknowledgedBy { get; set; }
    }
}