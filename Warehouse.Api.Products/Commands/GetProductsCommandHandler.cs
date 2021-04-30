using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Commands
{
    public record GetProductsCommand(Expression<Func<Product, bool>> Predicate) : IRequest<Result<List<ProductDto>>>;

    public class GetProductsCommandHandler : IRequestHandler<GetProductsCommand, Result<List<ProductDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetProductsCommandHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<Result<List<ProductDto>>> Handle(GetProductsCommand request, CancellationToken cancellationToken)
        {
            var productsInDb = await _productRepository.GetRangeAsync(request.Predicate);
            var products = _mapper.Map<List<ProductDto>>(productsInDb);

            return Result<List<ProductDto>>.Success(products);
        }
    }
}