using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Commands
{
    public record CreateCustomerCommand(Customer Customer) : IRequest;

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Task<Unit> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            _customerRepository.CreateAsync(request.Customer);
            return Unit.Task;
        }
    }
}