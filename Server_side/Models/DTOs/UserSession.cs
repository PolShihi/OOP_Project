namespace Server_side.Models.DTOs
{
    public record UserSession(string? Id, string? FirstName, string? LastName, string? PhoneNumber, string? Email, string? Role);
}
