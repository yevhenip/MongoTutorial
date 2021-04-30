using System.Threading.Tasks;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IFileService
    {
        Task WriteToFileAsync<T>(T item, string path, string fileName);
        
        Task<T> ReadFromFileAsync<T>(string path, string fileName);

        Task DeleteFileAsync(string path, string fileName);
    }
}