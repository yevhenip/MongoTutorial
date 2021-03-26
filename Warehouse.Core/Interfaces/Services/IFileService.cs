using System.Threading.Tasks;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Writes item to file system
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <typeparam name="T"></typeparam>
        Task WriteToFileAsync<T>(T item, string path, string fileName);
        
        /// <summary>
        /// Gets item from file system
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Item</returns>
        Task<T> ReadFromFileAsync<T>(string path, string fileName);

        /// <summary>
        /// Deletes file from file system
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        Task DeleteFileAsync(string path, string fileName);
    }
}