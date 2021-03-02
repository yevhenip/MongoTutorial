using System;
using AutoMapper;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Domain;

namespace Warehouse.Core.MapperProfile.ManufacturerProfile
{
    public class ManufacturerProfile : Profile
    {
        public ManufacturerProfile()
        {
            CreateMap<ManufacturerDto, Manufacturer>()
                .ForMember(m => m.Id, opt
                    => opt.MapFrom(m => m.Id))
                .ForMember(m => m.Name, opt
                    => opt.MapFrom(m => m.Name))
                .ForMember(m => m.Address, opt
                    => opt.MapFrom(m => m.Address));
            
            CreateMap<Manufacturer, ManufacturerDto>()
                .ForMember(m => m.Id, opt
                    => opt.MapFrom(m => m.Id))
                .ForMember(m => m.Name, opt
                    => opt.MapFrom(m => m.Name))
                .ForMember(m => m.Address, opt
                    => opt.MapFrom(m => m.Address));
            
            CreateMap<ManufacturerModelDto, ManufacturerDto>()
                .ForMember(m => m.Id, opt
                    => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(m => m.Name, opt
                    => opt.MapFrom(m => m.Name))
                .ForMember(m => m.Address, opt
                    => opt.MapFrom(m => m.Address));
        }
    }
}