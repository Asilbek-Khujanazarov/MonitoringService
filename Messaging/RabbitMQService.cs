using RabbitMQ.Client;
using System;
using System.Text;

namespace PatientRecoverySystem.MonitoringService.Messaging
{
    public class RabbitMQService : IRabbitMQService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "patient_recovery";

        public RabbitMQService(string hostName = "localhost")
        {
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
        }

        public void PublishMessage(string message, string routingKey)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }

        public void PublishVitalSignsAlert(string message)
        {
            PublishMessage(message, "monitoring.vitalsigns.alert");
        }

        public void PublishMonitorStatusUpdate(string message)
        {
            PublishMessage(message, "monitoring.status.updated");
        }

        public void PublishAlertAcknowledgement(string message)
        {
            PublishMessage(message, "monitoring.alert.acknowledged");
        }

        public void PublishMonitorCreated(string message)
        {
            PublishMessage(message, "monitoring.monitor.created");
        }

        public void PublishAlert(string message)
        {
            PublishMessage(message, "monitoring.alert");
        }

        public void PublishAlertAcknowledged(string message)
        {
            PublishMessage(message, "monitoring.alert.acknowledged");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}