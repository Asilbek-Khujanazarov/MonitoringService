namespace PatientRecovery.MonitoringService.Models
{
    public enum AlertType
    {
        HighTemperature,
        LowTemperature,
        HighHeartRate,
        LowHeartRate,
        HighBloodPressure,
        LowBloodPressure,
        LowOxygenSaturation,
        SystemAlert
    }

    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum MonitoringStatus
    {
        Active,
        Inactive,
        Maintenance,
        Error
    }
}