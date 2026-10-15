using UserManagement.Domain.Entities;

namespace UserManagement.Application.Ports.Out;

public sealed record AccessToken(string Value, int ExpiresIn);

public interface ITokenGenerator
{
    AccessToken Generate(User user);
}