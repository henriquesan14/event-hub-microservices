namespace Identity.Domain.Contracts;

public interface IPasswordCheck
{
    bool Matches(string password, string hash);
}
