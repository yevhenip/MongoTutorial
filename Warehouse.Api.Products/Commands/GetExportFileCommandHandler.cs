using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Products.Commands
{
    public record GetExportFileCommand : IRequest<Result<byte[]>>;

    public class GetExportFileCommandHandler : IRequestHandler<GetExportFileCommand, Result<byte[]>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetExportFileCommandHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<Result<byte[]>> Handle(GetExportFileCommand request, CancellationToken cancellationToken)
        {
            var productsInDb = await _productRepository.GetRangeAsync(_ => true);
            var exportProducts = _mapper.Map<List<ExportProduct>>(productsInDb);
            StringBuilder builder = new();
            foreach (var product in exportProducts)
            {
                builder.Append(product);
                builder.Append(Environment.NewLine);
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return Result<byte[]>.Success(bytes);
        }
    }
}