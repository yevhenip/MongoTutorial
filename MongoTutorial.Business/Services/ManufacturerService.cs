using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Manufacturer;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;

namespace MongoTutorial.Business.Services
{
    public class ManufacturerService : ServiceBase<Manufacturer>, IManufacturerService
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
            var manufacturersInDb = await _manufacturerRepository.GetAllAsync();
            var manufacturers = _mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds)
        {
            var manufacturersInDb =
                await _manufacturerRepository.GetRangeAsync(manufacturerIds);
            var manufacturers = _mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<ManufacturerDto>> GetAsync(string id)
        {
            var manufacturerInDb = await _manufacturerRepository.GetAsync(id);
            CheckForNull(manufacturerInDb);

            var manufacturer = _mapper.Map<ManufacturerDto>(manufacturerInDb);

            return Result<ManufacturerDto>.Success(manufacturer);
        }

        public async Task<Result<ManufacturerDto>> CreateAsync(ManufacturerModelDto manufacturer)
        {
            var manufacturerDto = _mapper.Map<ManufacturerDto>(manufacturer);
            var manufacturerToDb = _mapper.Map<Manufacturer>(manufacturerDto);

            await _manufacturerRepository.CreateAsync(manufacturerToDb);
            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<ManufacturerDto>> UpdateAsync(string id, ManufacturerModelDto manufacturer)
        {
            var manufacturerInDb = await _manufacturerRepository.GetAsync(id);
            CheckForNull(manufacturerInDb);

            var manufacturerDto = _mapper.Map<ManufacturerDto>(manufacturer) with {Id = id};
            manufacturerInDb = _mapper.Map<Manufacturer>(manufacturerDto);

            await _manufacturerRepository.UpdateAsync(manufacturerInDb);
            await UpdateManufacturesInProduct(manufacturerInDb.Id, manufacturerInDb);

            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var manufacturerInDb = await _manufacturerRepository.GetAsync(id);
            CheckForNull(manufacturerInDb);

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