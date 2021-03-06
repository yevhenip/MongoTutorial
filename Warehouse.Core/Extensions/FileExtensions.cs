using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Warehouse.Core.Extensions
{
    public static class FileExtensions
    {
        public static async Task WriteToFileAsync<T>(this T item, string path, string fileName)
        {
            await using var streamWriter = File.CreateText(path + fileName + ".json");
            await streamWriter.WriteAsync(JsonSerializer.Serialize(item));
        }

        public static async Task<T> ReadFromFileAsync<T>(string path, string fileName)
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

        public static Task DeleteFileAsync(string path, string fileName)
        {
            return Task.Run(() => File.Delete(path + fileName + ".json"));
        }
    }
}