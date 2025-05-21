using AutoMapper;
using PatientRecovery.MonitoringService.DTOs;
using PatientRecovery.MonitoringService.Models;

namespace PatientRecoverySystem.MonitoringService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VitalSignsMonitor, MonitorDto>();
            CreateMap<VitalSignsAlert, AlertDto>();
        }
    }
}