using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using ISender = Warehouse.Core.Interfaces.Services.ISender;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Warehouse.Api.Users.Commands
{
    public record UpdateUserCommand(string UserId, UserModelDto User, string UserName) : IRequest<Result<UserDto>>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly ICacheService _cacheService;
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly CacheUserSettings _userSettings;

        public UpdateUserCommandHandler(IMapper mapper, ISender sender, ICacheService cacheService,
            IFileService fileService, IOptions<CacheUserSettings> userSettings, IUserRepository userRepository,
            IRefreshTokenRepository tokenRepository)
        {
            _mapper = mapper;
            _sender = sender;
            _cacheService = cacheService;
            _fileService = fileService;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _userSettings = userSettings.Value;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"User-{request.UserId}";
            User userInDb;
            if (!await _cacheService.IsExistsAsync(cacheKey))
            {
                userInDb = await _userRepository.GetAsync(u => u.Id == request.UserId) ??
                           await _fileService.ReadFromFileAsync<User>(CommandExtensions.UserFolderPath, cacheKey);
                userInDb.CheckForNull();
            }

            var userDto = _mapper.Map<UserDto>(request.User) with {Id = request.UserId};
            userInDb = _mapper.Map<User>(userDto);
            var token = await _tokenRepository.GetAsync(t => t.User.Id == request.UserId);
            LogDto log =
                new(Guid.NewGuid().ToString(), request.UserName, "edited user", JsonSerializer.Serialize(userDto,
                    CommandExtensions.JsonSerializerOptions), DateTime.UtcNow);

            if (token is not null)
            {
                token.User = userInDb;
                await _tokenRepository.UpdateAsync(t => t.Id == token.Id, token);
            }

            await _userRepository.UpdateAsync(u => u.Id == userInDb.Id, userInDb);
            await _cacheService.UpdateAsync(cacheKey, userInDb, _userSettings);
            await _fileService.WriteToFileAsync(userInDb, CommandExtensions.UserFolderPath, cacheKey);
            await _sender.PublishAsync(log, cancellationToken);

            return Result<UserDto>.Success(userDto);
        }
    }
}