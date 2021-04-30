using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Products.Commands
{
    public record DeleteCustomerFromProductCommand(string Id) : IRequest;

    public class DeleteCustomerFromProductCommandHandler : IRequestHandler<DeleteCustomerFromProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomerFromProductCommandHandler(IProductRepository productRepository,
            ICustomerRepository customerRepository)
        {
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Unit> Handle(DeleteCustomerFromProductCommand request, CancellationToken cancellationToken)
        {
            var products =
                await _productRepository.GetRangeAsync(p => p.Customer.Id == request.Id);
            foreach (var product in products)
            {
                product.Customer = null;
                await _productRepository.UpdateAsync(p => p.Id == product.Id, product);
            }

            await _customerRepository.DeleteAsync(c => c.Id == request.Id);
            return Unit.Value;
        }
    }
}