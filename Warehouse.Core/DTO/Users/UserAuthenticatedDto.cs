namespace Warehouse.Core.DTO.Users
{
    public record UserAuthenticatedDto(UserDto User, string JwtToken, string RefreshToken);
}