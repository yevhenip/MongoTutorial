using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using ISender = Warehouse.Core.Interfaces.Services.ISender;

namespace Warehouse.Api.Customers.Commands
{
    public record DeleteCustomerCommand(string Id) : IRequest<Result<object>>;

    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<object>>
    {
        private readonly ICacheService _cacheService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISender _sender;

        public DeleteCustomerCommandHandler(ICacheService cacheService, ISender sender,
            ICustomerRepository customerRepository)
        {
            _cacheService = cacheService;
            _customerRepository = customerRepository;
            _sender = sender;
        }

        public async Task<Result<object>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Customer-{request.Id}";
            if (!await _cacheService.IsExistsAsync(cacheKey))
            {
                var customerInDb = await _customerRepository.GetAsync(c => c.Id == request.Id);
                customerInDb.CheckForNull();
            }

            await _sender.PublishAsync(new DeletedCustomer(request.Id), cancellationToken);
            await _cacheService.RemoveAsync(cacheKey);
            await _customerRepository.DeleteAsync(c => c.Id == request.Id);

            return Result<object>.Success();
        }
    }
}