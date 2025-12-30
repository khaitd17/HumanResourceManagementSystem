using HRM.ServiceLayer.DTOs.Auth;
using HRM.ServiceLayer.DTOs.Common;

namespace HRM.ServiceLayer.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto request);
    Task<ServiceResult<LoginResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<ServiceResult> LogoutAsync(int userId);
}
