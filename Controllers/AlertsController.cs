using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PatientRecovery.MonitoringService.Services;
using PatientRecovery.MonitoringService.DTOs;
using PatientRecoverySystem.MonitoringService.Messaging;
using PatientRecovery.MonitoringService.Services;

namespace PatientRecoverySystem.MonitoringService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IVitalSignsMonitoringService _monitoringService;
        private readonly IMapper _mapper;
        private readonly IRabbitMQService _messageBus;

        public AlertsController(
            IVitalSignsMonitoringService monitoringService,
            IMapper mapper,
            IRabbitMQService messageBus)
        {
            _monitoringService = monitoringService;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        [HttpGet("monitor/{monitorId}")]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAlertsByMonitor(Guid monitorId)
        {
            var alerts = await _monitoringService.GetAlertsByMonitorIdAsync(monitorId);
            return Ok(_mapper.Map<IEnumerable<AlertDto>>(alerts));
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAlertsByPatient(Guid patientId)
        {
            var alerts = await _monitoringService.GetAlertsByPatientIdAsync(patientId);
            return Ok(_mapper.Map<IEnumerable<AlertDto>>(alerts));
        }

        [HttpPost("{alertId}/acknowledge")]
        public async Task<IActionResult> AcknowledgeAlert(Guid alertId, [FromBody] AcknowledgeAlertRequest request)
        {
            await _monitoringService.AcknowledgeAlertAsync(alertId, request.AcknowledgedBy);
            _messageBus.PublishAlertAcknowledged(JsonSerializer.Serialize(new { AlertId = alertId, AcknowledgedBy = request.AcknowledgedBy }));
            return NoContent();
        }
    }
}