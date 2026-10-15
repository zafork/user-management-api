using UserManagement.Application.DTOs;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Ports.In;
using UserManagement.Application.Ports.Out;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.UseCases;

public sealed class LoginUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator) : ILoginUseCase
{
    private const string BearerTokenType = "Bearer";

    public async Task<TokenResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = User.NormalizeEmail(request.Email);
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        var token = tokenGenerator.Generate(user);
        return new TokenResponse(token.Value, BearerTokenType, token.ExpiresIn);
    }
}