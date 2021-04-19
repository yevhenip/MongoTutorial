using System;
using AutoMapper;
using Warehouse.Core.DTO.Log;
using Warehouse.Domain;

namespace Warehouse.Core.MapperProfile.LogProfile
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<Log, LogDto>()
                .ForMember(l => l.Id, opt
                    => opt.MapFrom(l => l.Id))
                .ForMember(l => l.UserName, opt
                    => opt.MapFrom(l => l.UserName))
                .ForMember(l => l.Action, opt
                    => opt.MapFrom(l => l.Action))
                .ForMember(l => l.SerializedData, opt
                    => opt.MapFrom(l => l.SerializedData))
                .ForMember(l => l.ActionDate, opt
                    => opt.MapFrom(l => l.ActionDate));

            CreateMap<LogDto, Log>()
                .ForMember(l => l.Id, opt
                    => opt.MapFrom(l => l.Id ?? Guid.NewGuid().ToString()))
                .ForMember(l => l.UserName, opt
                    => opt.MapFrom(l => l.UserName))
                .ForMember(l => l.Action, opt
                    => opt.MapFrom(l => l.Action))
                .ForMember(l => l.SerializedData, opt
                    => opt.MapFrom(l => l.SerializedData))
                .ForMember(l => l.ActionDate, opt
                    => opt.MapFrom(l => DateTime.UtcNow));
        }
    }
}