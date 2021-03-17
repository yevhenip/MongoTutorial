using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Business;
using Warehouse.Domain;

namespace Warehouse.Api.Tests.Business
{
    [TestFixture]
    public class FileServiceTests
    {
        private readonly FileService _fileService = new();

        [Test]
        public async Task WriteToFileAsync_WhenCalled_WritesFile()
        {
            await _fileService.WriteToFileAsync(new User(), It.IsAny<string>(), It.IsAny<string>());
        }
        
        
        [Test]
        public async Task DeleteFileAsync_WhenCalled_DeletesFile()
        {
            await _fileService.DeleteFileAsync(It.IsAny<string>(), It.IsAny<string>());
        }
    }
}