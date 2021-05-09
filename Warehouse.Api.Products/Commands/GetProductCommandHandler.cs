using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Commands
{
    public record GetProductCommand(string Id) : IRequest<Result<ProductDto>>;

    public class GetProductCommandHandler : IRequestHandler<GetProductCommand, Result<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IProductRepository _productRepository;
        private readonly CacheProductSettings _productSettings;

        public GetProductCommandHandler(IMapper mapper, ICacheService cacheService,
            IProductRepository productRepository, IOptions<CacheProductSettings> productSettings)
        {
            _mapper = mapper;
            _cacheService = cacheService;
            _productRepository = productRepository;
            _productSettings = productSettings.Value;
        }

        public async Task<Result<ProductDto>> Handle(GetProductCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Product-{request.Id}";
            var cache = await _cacheService.GetStringAsync(cacheKey);
            ProductDto product;

            if (cache.TryGetValue<Product>(out var cachedProduct))
            {
                product = _mapper.Map<ProductDto>(cachedProduct);

                return Result<ProductDto>.Success(product);
            }

            var productInDb = await _productRepository.GetAsync(p => p.Id == request.Id);

            productInDb.CheckForNull();
            await _cacheService.SetCacheAsync(cacheKey, productInDb, _productSettings);

            product = _mapper.Map<ProductDto>(productInDb);

            return Result<ProductDto>.Success(product);
        }
    }
}