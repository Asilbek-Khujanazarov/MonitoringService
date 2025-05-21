using System;
using System.Collections.Generic;

namespace PatientRecovery.MonitoringService.Models
{
    public class VitalSignsMonitor
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
        public bool IsDeleted { get; set; }

        public virtual ICollection<VitalSignsAlert> Alerts { get; set; }
    }
}