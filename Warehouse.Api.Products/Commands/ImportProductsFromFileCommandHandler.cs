using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Product;

namespace Warehouse.Api.Products.Commands
{
    public record ImportProductsFromFileCommand(IFormFile File, string UserName) : IRequest<Result<string>>;

    public class ImportProductsFromFileCommandHandler : IRequestHandler<ImportProductsFromFileCommand, Result<string>>
    {
        private readonly IMediator _mediator;

        public ImportProductsFromFileCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<string>> Handle(ImportProductsFromFileCommand request,
            CancellationToken cancellationToken)
        {
            var products = await GetDataFromFileAsync(request.File);
            var errors = await CreateProducts(products, request.UserName, cancellationToken);
            var result = GetErrorString(errors);

            return Result<string>.Success(result);
        }

        private static async Task<List<ProductModelDto>> GetDataFromFileAsync(IFormFile file)
        {
            List<ProductModelDto> products = new();

            var reader = new StreamReader(file.OpenReadStream());
            if (Path.GetExtension(file.FileName) == ".csv")
            {
                var i = 1;
                while (!reader.EndOfStream)
                {
                    var data = await reader.ReadLineAsync();
                    var dataArray = data?.Split(',');
                    if (dataArray is not {Length: 4})
                    {
                        throw Result<IFormFile>.Failure("row", $"Cannot parse row number {i}");
                    }

                    var productName = dataArray[0];
                    if (!DateTime.TryParse(dataArray[1], out var date))
                    {
                        throw Result<IFormFile>.Failure("date", $"Cannot parse date at {i} row");
                    }

                    List<string> manufacturerIds = new();
                    if (dataArray[2] != "none")
                    {
                        var manufacturerIdsData = dataArray[2].Split(';');
                        manufacturerIds.AddRange(manufacturerIdsData);
                    }

                    var customerId = dataArray[3] != "none" ? dataArray[3] : null;
                    products.Add(new ProductModelDto(productName, date, manufacturerIds, customerId));
                    ++i;
                }
            }
            else
            {
                var data = await reader.ReadToEndAsync();
                try
                {
                    products = JsonSerializer.Deserialize<List<ProductModelDto>>(data);
                }
                catch (Exception)
                {
                    throw Result<IFormFile>.Failure("date", "Cannot parse your json file");
                }
            }

            return products;
        }

        private async Task<List<int>> CreateProducts(IEnumerable<ProductModelDto> products, string userName,
            CancellationToken cancellationToken)
        {
            List<int> errors = new();
            var i = 1;
            foreach (var product in products)
            {
                try
                {
                    await _mediator.Send(new CreateProductCommand(product, userName), cancellationToken);
                }
                catch (Exception)
                {
                    errors.Add(i);
                }

                ++i;
            }

            return errors;
        }

        private static string GetErrorString(IReadOnlyCollection<int> errors)
        {
            if (!errors.Any())
            {
                return "";
            }

            StringBuilder builder = new();
            builder.Append("Product in rows number ");
            foreach (var error in errors)
            {
                builder.Append($"{error}, ");
            }

            builder.Append("not added due to error, please check this row and check existing of provided ids");
            return builder.ToString();
        }
    }
}