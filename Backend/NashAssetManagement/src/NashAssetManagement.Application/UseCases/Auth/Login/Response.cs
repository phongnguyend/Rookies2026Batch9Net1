namespace NashAssetManagement.Application.UseCases.Auth.Login
{
    public record Response(string AccessToken, string RefreshToken);
}