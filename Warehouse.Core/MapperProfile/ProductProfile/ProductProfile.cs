using System;
using AutoMapper;
using Warehouse.Core.DTO.Product;
using Warehouse.Domain;

namespace Warehouse.Core.MapperProfile.ProductProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductDto, Product>()
                .ForMember(p => p.Id, opt
                    => opt.MapFrom(p => p.Id))
                .ForMember(p => p.Name, opt
                    => opt.MapFrom(p => p.Name))
                .ForMember(p => p.DateOfReceipt, opt
                    => opt.MapFrom(p => p.DateOfReceipt))
                .ForMember(p => p.Manufacturers, opt
                    => opt.MapFrom(p => p.Manufacturers))
                .ForMember(p => p.Customer, opt
                    => opt.MapFrom(p => p.Customer));

            CreateMap<Product, ProductDto>()
                .ForMember(p => p.Id, opt
                    => opt.MapFrom(p => p.Id))
                .ForMember(p => p.Name, opt
                    => opt.MapFrom(p => p.Name))
                .ForMember(p => p.DateOfReceipt, opt
                    => opt.MapFrom(p => p.DateOfReceipt))
                .ForMember(p => p.Manufacturers, opt
                    => opt.MapFrom(p => p.Manufacturers))
                .ForMember(p => p.Customer, opt
                    => opt.MapFrom(p => p.Customer));

            CreateMap<ProductModelDto, Product>()
                .ForMember(p => p.Id, opt
                    => opt.MapFrom(_ => Guid.NewGuid().ToString()))
                .ForMember(p => p.Name, opt
                    => opt.MapFrom(p => p.Name))
                .ForMember(p => p.DateOfReceipt, opt
                    => opt.MapFrom(p => p.DateOfReceipt));
        }
    }
}