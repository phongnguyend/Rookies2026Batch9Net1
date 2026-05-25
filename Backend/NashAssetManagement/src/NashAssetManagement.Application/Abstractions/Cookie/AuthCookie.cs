namespace NashAssetManagement.Application.Abstractions.Cookie
{
    public sealed record AuthCookie(string TokenName, string Value, DateTime ExpiresAtUtc, bool HttpOnly, bool Secure, string Path, bool IsEssential);
}