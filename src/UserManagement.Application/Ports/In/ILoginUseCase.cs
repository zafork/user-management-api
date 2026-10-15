using UserManagement.Application.DTOs;

namespace UserManagement.Application.Ports.In;

public interface ILoginUseCase
{
    Task<TokenResponse> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default);
}