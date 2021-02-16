using System;
using AutoMapper;
using MongoTutorial.Api.Models.Manufacturer;
using MongoTutorial.Core.Dtos;

namespace MongoTutorial.Api.MapperProfile.ManufacturerProfile
{
    public class ManufacturerProfile : Profile
    {
        public ManufacturerProfile()
        {
            CreateMap<ManufacturerModel, ManufacturerDto>()
                .ForMember(m => m.Id, opt
                    => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(m => m.Name, opt
                    => opt.MapFrom(m => m.Name))
                .ForMember(m => m.Address, opt
                    => opt.MapFrom(m => m.Address));
        }
    }
}