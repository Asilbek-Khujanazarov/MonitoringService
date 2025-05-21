using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PatientRecovery.MonitoringService.DTOs;
using PatientRecovery.MonitoringService.Services;
using PatientRecovery.MonitoringService.Models;
using AutoMapper;

using System.Text.Json;
using PatientRecoverySystem.MonitoringService.Messaging;

namespace PatientRecoverySystem.MonitoringService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitorsController : ControllerBase
    {
        private readonly IVitalSignsMonitoringService _monitoringService;
        private readonly IMapper _mapper;
        private readonly IRabbitMQService _messageBus;

        public MonitorsController(
            IVitalSignsMonitoringService monitoringService,
            IMapper mapper,
            IRabbitMQService messageBus)
        {
            _monitoringService = monitoringService;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        [HttpPost]
        public async Task<ActionResult<MonitorDto>> CreateMonitor([FromBody] CreateMonitorRequest request)
        {
            var monitor = new VitalSignsMonitor
            {
                PatientId = request.PatientId,
                DeviceId = request.DeviceId,
                Status = MonitoringStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var createdMonitor = await _monitoringService.CreateMonitorAsync(monitor);
            _messageBus.PublishMonitorCreated(JsonSerializer.Serialize(createdMonitor));

            return CreatedAtAction(nameof(GetMonitor), new { id = createdMonitor.Id }, _mapper.Map<MonitorDto>(createdMonitor));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MonitorDto>> GetMonitor(Guid id)
        {
            var monitor = await _monitoringService.GetMonitorByIdAsync(id);
            if (monitor == null)
                return NotFound();

            return Ok(_mapper.Map<MonitorDto>(monitor));
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<MonitorDto>> GetPatientMonitor(Guid patientId)
        {
            var monitor = await _monitoringService.GetMonitorByPatientIdAsync(patientId);
            if (monitor == null)
                return NotFound();

            return Ok(_mapper.Map<MonitorDto>(monitor));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateMonitorStatus(Guid id, [FromBody] UpdateMonitorStatusRequest request)
        {
            await _monitoringService.UpdateMonitorStatusAsync(id, request.Status);
            return NoContent();
        }

        [HttpPost("{id}/vitals")]
        public async Task<IActionResult> UpdateVitalSigns(Guid id, [FromBody] UpdateVitalSignsRequest request)
        {
            var monitor = await _monitoringService.GetMonitorByIdAsync(id);
            if (monitor == null)
                return NotFound();

            monitor.Temperature = request.Temperature;
            monitor.HeartRate = request.HeartRate;
            monitor.BloodPressureSystolic = request.BloodPressureSystolic;
            monitor.BloodPressureDiastolic = request.BloodPressureDiastolic;
            monitor.RespiratoryRate = request.RespiratoryRate;
            monitor.OxygenSaturation = request.OxygenSaturation;
            monitor.UpdatedAt = DateTime.UtcNow;

            await _monitoringService.UpdateMonitorAsync(monitor);

            var alert = await _monitoringService.ProcessVitalSignsAsync(monitor);
            if (alert != null)
            {
                _messageBus.PublishAlert(JsonSerializer.Serialize(alert));
            }

            return NoContent();
        }
    }
}