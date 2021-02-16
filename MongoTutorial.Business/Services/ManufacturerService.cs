using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;

namespace MongoTutorial.Business.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ManufacturerService(IManufacturerRepository manufacturerRepository, IProductRepository productRepository,
            IMapper mapper)
        {
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ManufacturerDto>>> GetAllAsync()
        {
            var manufacturersFromDb = await _manufacturerRepository.GetAllAsync();
            var manufacturers = _mapper.Map<List<ManufacturerDto>>(manufacturersFromDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds)
        {
            var manufacturersFromDb =
                await _manufacturerRepository.GetRangeAsync(manufacturerIds);
            var manufacturers = _mapper.Map<List<ManufacturerDto>>(manufacturersFromDb);
            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<ManufacturerDto>> GetAsync(string id)
        {
            var manufacturerFromDb = await _manufacturerRepository.GetAsync(id);
            if (manufacturerFromDb == null)
            {
                throw Result<ManufacturerDto>.Failure("manufacturerId", "Manufacturer doesn't exists",
                    HttpStatusCode.BadRequest);
            }

            var manufacturer = _mapper.Map<ManufacturerDto>(manufacturerFromDb);

            return Result<ManufacturerDto>.Success(manufacturer);
        }

        public async Task<Result<ManufacturerDto>> CreateAsync(ManufacturerDto manufacturer)
        {
            var manufacturerToDb = _mapper.Map<Manufacturer>(manufacturer);
            await _manufacturerRepository.CreateAsync(manufacturerToDb);
            return Result<ManufacturerDto>.Success(manufacturer);
        }

        public async Task<Result<ManufacturerDto>> UpdateAsync(ManufacturerDto manufacturer)
        {
            var manufacturerFromDb = await _manufacturerRepository.GetAsync(manufacturer.Id);
            if (manufacturerFromDb == null)
            {
                throw Result<ManufacturerDto>.Failure("manufacturerId", "Manufacturer doesn't exists",
                    HttpStatusCode.BadRequest);
            }

            var manufacturerToDb = _mapper.Map<Manufacturer>(manufacturer);
            await _manufacturerRepository.UpdateAsync(manufacturerToDb);
            
            await UpdateManufacturesInProduct(manufacturerToDb.Id, manufacturerToDb);
            
            return Result<ManufacturerDto>.Success(manufacturer);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var manufacturerFromDb = await _manufacturerRepository.GetAsync(id);
            if (manufacturerFromDb == null)
            {
                throw Result<ManufacturerDto>.Failure("manufacturerId", "Manufacturer doesn't exists",
                    HttpStatusCode.BadRequest);
            }

            await UpdateManufacturesInProduct(id);

            await _manufacturerRepository.DeleteAsync(id);
            return Result<object>.Success();
        }

        private async Task UpdateManufacturesInProduct(string id, Manufacturer manufacturer = null)
        {
            var products = await _productRepository.GetRangeByManufacturerId(id);
            foreach (var product in products)
            {
                var manufacturers = product.Manufacturers.ToList();
                manufacturers.RemoveAll(m => m.Id == id);
                
                manufacturers.Add(manufacturer);
                product.Manufacturers = manufacturers;
                
                await _productRepository.UpdateAsync(product);
            }
        }
    }
}