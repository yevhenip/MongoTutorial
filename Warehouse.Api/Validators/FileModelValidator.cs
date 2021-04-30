using FluentValidation;
using Microsoft.Extensions.Options;
using Warehouse.Core;
using Warehouse.Core.Settings;

namespace Warehouse.Api.Validators
{
    public class FileModelValidator : AbstractValidator<FileModel>
    {
        private readonly FileSettings _settings;

        public FileModelValidator(IOptions<FileSettings> fileSettings)
        {
            _settings = fileSettings.Value;

            RuleFor(f => f.File).NotEmpty()
                .WithMessage("File is required");
            
            RuleFor(f => f.File.Length).ExclusiveBetween(0, _settings.MaxBytes)
                .WithMessage(
                    $"File length should be greater than 0 and less than {_settings.MaxBytes / 1024 / 1024} MB")
                .When(p => p.File != null);

            RuleFor(f => f.File.FileName).Must(HaveSupportedFileType)
                .WithMessage("Unsupported file extension!")
                .When(p => p.File != null);
        }

        private bool HaveSupportedFileType(string fileName)
        {
            return _settings.IsSupported(fileName);
        }
    }
}