namespace Derrick.Personal.Server.Repository.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(string email);
}