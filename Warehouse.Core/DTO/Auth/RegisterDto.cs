namespace Warehouse.Core.DTO.Auth
{
    public record RegisterDto(string UserName, string FullName, string Password, string ConfirmedPassword, string Email,
        string Phone);
}