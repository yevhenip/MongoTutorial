using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Business
{
    public class FileService : IFileService
    {
        public async Task WriteToFileAsync<T>(T item, string path, string fileName)
        {
            await using var streamWriter = File.CreateText(path + fileName + ".json");
            await streamWriter.WriteAsync(JsonSerializer.Serialize(item));
        }

        public async Task<T> ReadFromFileAsync<T>(string path, string fileName)
        {
            if (Directory.GetFiles(path).All(p => p != path + fileName + ".json"))
            {
                return default;
            }

            using var streamReader = File.OpenText(path + fileName + ".json");
            var s = await streamReader.ReadToEndAsync();
            var item = JsonSerializer.Deserialize<T>(s);
            return item;
        }

        public Task DeleteFileAsync(string path, string fileName)
        {
            return Task.Run(() => File.Delete(path + fileName + ".json"));
        }
    }
}