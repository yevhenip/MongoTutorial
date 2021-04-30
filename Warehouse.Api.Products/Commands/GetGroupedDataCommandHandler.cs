using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Products.Commands
{
    public record GetGroupedDataCommand : IRequest<Result<List<GroupData>>>;

    public class GetGroupedDataCommandHandler : IRequestHandler<GetGroupedDataCommand, Result<List<GroupData>>>
    {
        private readonly IProductRepository _productRepository;

        public GetGroupedDataCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<List<GroupData>>> Handle(GetGroupedDataCommand request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GroupByAsync(p => p.Name);
            return Result<List<GroupData>>.Success(result);
        }
    }
}