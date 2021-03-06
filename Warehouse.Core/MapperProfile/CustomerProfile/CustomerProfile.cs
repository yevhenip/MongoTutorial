using System;
using AutoMapper;
using Warehouse.Core.DTO.Customer;
using Warehouse.Domain;

namespace Warehouse.Core.MapperProfile.CustomerProfile
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(c => c.Id, opt
                    => opt.MapFrom(c => c.Id))
                .ForMember(c => c.FullName, opt
                    => opt.MapFrom(c => c.FullName))
                .ForMember(c => c.Email, opt
                    => opt.MapFrom(c => c.Email))
                .ForMember(c => c.Phone, opt
                    => opt.MapFrom(c => c.Phone));
            
            CreateMap<CustomerDto, Customer>()
                .ForMember(c => c.Id, opt
                    => opt.MapFrom(c => Guid.NewGuid().ToString()))
                .ForMember(c => c.FullName, opt
                    => opt.MapFrom(c => c.FullName))
                .ForMember(c => c.Email, opt
                    => opt.MapFrom(c => c.Email))
                .ForMember(c => c.Phone, opt
                    => opt.MapFrom(c => c.Phone));
        }
    }
}