namespace Server_side.Models.DTOs
{
    public class ServiceResponces
    {
        public record class GeneralResponse(bool Flag, string Message);
        public record class LoginResponse(bool Flag, string Token, string Message);
    }
}
