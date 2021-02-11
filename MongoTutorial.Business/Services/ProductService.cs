using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;

namespace MongoTutorial.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var productsFromDb = await _productRepository.GetProductsAsync();
            var products = productsFromDb.Select(p => new ProductDto(p.Id, p.Name, p.DateOfReceipt)).ToList();
            return products;
        }

        public async Task<ProductDto> GetProductByIdAsync(string id)
        {
            var productFromDb = await _productRepository.GetProductByIdAsync(id);
            ProductDto product = new(productFromDb.Id, productFromDb.Name, productFromDb.DateOfReceipt);
            return product;
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto product)
        {
            Product productToDb = new() {Id = product.Id, Name = product.Name, DateOfReceipt = product.DateOfReceipt};
            await _productRepository.CreateProductAsync(productToDb);
            return product;
        }

        public async Task UpdateProductAsync(ProductDto product)
        {
            Product productToDb = new() {Id = product.Id, Name = product.Name, DateOfReceipt = product.DateOfReceipt};
            await _productRepository.UpdateProductAsync(productToDb);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _productRepository.DeleteProductAsync(id);
        }
    }
}