using AutoMapper;
using MongoTutorial.Core.Dtos;
using MongoTutorial.Domain;

namespace MongoTutorial.Core.MapperProfile.ProductProfile
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
                    => opt.MapFrom(p => p.DateOfReceipt));
            
            CreateMap<Product, ProductDto>()
                .ForMember(p => p.Id, opt
                    => opt.MapFrom(p => p.Id))
                .ForMember(p => p.Name, opt
                    => opt.MapFrom(p => p.Name))
                .ForMember(p => p.DateOfReceipt, opt
                    => opt.MapFrom(p => p.DateOfReceipt));
        }
    }
}