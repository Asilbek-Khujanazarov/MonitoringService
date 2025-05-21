namespace PatientRecoverySystem.MonitoringService.Messaging
{
    public interface IRabbitMQService
    {
        void PublishMessage(string message, string routingKey);
        void PublishVitalSignsAlert(string message);
        void PublishMonitorStatusUpdate(string message);
        void PublishAlertAcknowledgement(string message);
        void PublishMonitorCreated(string message);
        void PublishAlert(string message);
        void PublishAlertAcknowledged(string message);
    }
}