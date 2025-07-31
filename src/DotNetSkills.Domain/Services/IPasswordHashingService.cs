namespace DotNetSkills.Domain.Services;

public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    bool IsValidPasswordHash(string passwordHash);
}