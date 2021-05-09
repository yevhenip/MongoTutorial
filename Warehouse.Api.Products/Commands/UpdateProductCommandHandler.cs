using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using ISender = Warehouse.Core.Interfaces.Services.ISender;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Warehouse.Api.Products.Commands
{
    public record UpdateProductCommand(string ProductId, ProductModelDto Product, string UserName)
        : IRequest<Result<ProductDto>>;

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly ICacheService _cacheService;
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;
        private readonly CacheProductSettings _productSettings;

        public UpdateProductCommandHandler(IMapper mapper, ISender sender, ICacheService cacheService,
            IProductRepository productRepository, IProductService productService,
            IOptions<CacheProductSettings> productSettings)
        {
            _mapper = mapper;
            _sender = sender;
            _cacheService = cacheService;
            _productRepository = productRepository;
            _productService = productService;
            _productSettings = productSettings.Value;
        }

        public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Product-{request.ProductId}";
            Product productInDb;
            if (!await _cacheService.IsExistsAsync(cacheKey))
            {
                productInDb = await _productRepository.GetAsync(p => p.Id == request.ProductId);

                productInDb.CheckForNull();
            }

            productInDb = await _productService.GetManufacturersAndCustomer(request.Product,
                request.Product.ManufacturerIds.ToList());
            productInDb.Id = request.ProductId;
            var result = _mapper.Map<ProductDto>(productInDb) with {Id = request.ProductId};
            LogDto log = new(Guid.NewGuid().ToString(), request.UserName, "edited product",
                JsonSerializer.Serialize(result, CommandExtensions.JsonSerializerOptions), DateTime.UtcNow);

            await _cacheService.SetCacheAsync(cacheKey, productInDb, _productSettings);
            await _sender.PublishAsync(log, cancellationToken);
            await _productRepository.UpdateAsync(p => p.Id == productInDb.Id, productInDb);
            return Result<ProductDto>.Success(result);
        }
    }
}