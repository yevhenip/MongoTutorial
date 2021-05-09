using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Commands
{
    public record DeleteProductCommand(string Id) : IRequest<Result<object>>;

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<object>>
    {
        private readonly ICacheService _cacheService;
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(ICacheService cacheService, IProductRepository productRepository)
        {
            _cacheService = cacheService;
            _productRepository = productRepository;
        }

        public async Task<Result<object>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Product-{request.Id}";
            if (!await _cacheService.IsExistsAsync(cacheKey))
            {
                var productInDb = await _productRepository.GetAsync(p => p.Id == request.Id);
                productInDb.CheckForNull();
            }

            await _productRepository.DeleteAsync(p => p.Id == request.Id);
            await _cacheService.RemoveAsync(cacheKey);

            return Result<object>.Success();
        }
    }
}