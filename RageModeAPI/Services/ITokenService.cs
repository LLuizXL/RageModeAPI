namespace RageModeAPI.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(Models.Usuarios user);
    }
}
