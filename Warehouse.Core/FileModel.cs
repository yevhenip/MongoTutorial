using Microsoft.AspNetCore.Http;

namespace Warehouse.Core
{
    public class FileModel
    {
        public IFormFile File { get; set; }
    }
}