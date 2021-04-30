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
using ISender = Warehouse.Core.Interfaces.Services.ISender;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Warehouse.Api.Products.Commands
{
    public record CreateProductCommand(ProductModelDto Product, string UserName) : IRequest<Result<ProductDto>>;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly ICacheService _cacheService;
        private readonly IFileService _fileService;
        private readonly IProductRepository _productRepository;
        private readonly CacheProductSettings _productSettings;
        private readonly IProductService _productService;

        public CreateProductCommandHandler(IMapper mapper, ISender sender, ICacheService cacheService,
            IFileService fileService, IProductRepository productRepository, IProductService productService,
            IOptions<CacheProductSettings> productSettings)
        {
            _mapper = mapper;
            _sender = sender;
            _cacheService = cacheService;
            _fileService = fileService;
            _productRepository = productRepository;
            _productSettings = productSettings.Value;
            _productService = productService;
        }

        public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productToDb =
                await _productService.GetManufacturersAndCustomer(request.Product,
                    request.Product.ManufacturerIds.ToList());

            var cacheKey = $"Product-{productToDb.Id}";
            await _cacheService.SetCacheAsync(cacheKey, productToDb, _productSettings);
            await _fileService.WriteToFileAsync(productToDb, CommandExtensions.ProductFolderPath, cacheKey);

            var result = _mapper.Map<ProductDto>(productToDb);
            LogDto log = new(Guid.NewGuid().ToString(), request.UserName, "added product",
                JsonSerializer.Serialize(result, CommandExtensions.JsonSerializerOptions), DateTime.UtcNow);

            await _sender.PublishAsync(log, cancellationToken);
            await _productRepository.CreateAsync(productToDb);
            return Result<ProductDto>.Success(result);
        }
    }
}