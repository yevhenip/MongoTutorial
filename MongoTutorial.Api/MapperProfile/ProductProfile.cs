using System;
using AutoMapper;
using MongoTutorial.Api.Models.Product;
using MongoTutorial.Core.Dtos;

namespace MongoTutorial.Api.MapperProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductModel, ProductDto>()
                .ForMember(p => p.Id, opt
                    => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(p => p.Name, opt
                    => opt.MapFrom(p => p.Name))
                .ForMember(p => p.DateOfReceipt, opt
                    => opt.MapFrom(p => p.DateOfReceipt));
        }
    }
}