using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Products.Commands
{
    public record GetProductsPageCommand(int Page, int PageSize) : IRequest<Result<PageDataDto<ProductDto>>>;

    public class GetProductsPageCommandHandler :
        IRequestHandler<GetProductsPageCommand, Result<PageDataDto<ProductDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetProductsPageCommandHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<Result<PageDataDto<ProductDto>>> Handle(GetProductsPageCommand request,
            CancellationToken cancellationToken)
        {
            var productsInDb = await _productRepository.GetPageAsync(request.Page, request.PageSize);
            var count = await _productRepository.GetCountAsync(_ => true);
            var products = _mapper.Map<List<ProductDto>>(productsInDb);
            PageDataDto<ProductDto> pageData = new(products, count);

            return Result<PageDataDto<ProductDto>>.Success(pageData);
        }
    }
}