namespace Derrick.Personal.Repository.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(string email);
}