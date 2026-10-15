namespace UserManagement.Application.DTOs;

public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);