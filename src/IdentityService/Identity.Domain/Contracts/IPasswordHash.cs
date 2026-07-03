namespace Identity.Domain.Contracts;

public interface IPasswordHash
{
    string HashPassword(string password);
}
