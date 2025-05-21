using System;

namespace PatientRecoverySystem.MonitoringService.Models.Request
{
    public class MonitoringRequest
    {
        public Guid PatientId { get; set; }
        public required string DeviceId { get; set; }
    }
}