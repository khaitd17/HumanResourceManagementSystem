using AutoMapper;
using HRM.DataLayer.Entities;
using HRM.RepositoryLayer.Interfaces;
using HRM.ServiceLayer.DTOs.Auth;
using HRM.ServiceLayer.DTOs.Common;
using HRM.ServiceLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HRM.ServiceLayer.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            Console.WriteLine($"[DEBUG] Login attempt for username: {request.Username}");
            
            var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
            
            Console.WriteLine($"[DEBUG] User found: {user != null}");
            
            if (user != null)
            {
                Console.WriteLine($"[DEBUG] User.Username: {user.Username}");
                Console.WriteLine($"[DEBUG] User.IsActive: {user.IsActive}");
                Console.WriteLine($"[DEBUG] User.PasswordHash length: {user.PasswordHash?.Length ?? 0}");
                Console.WriteLine($"[DEBUG] User.PasswordHash: {user.PasswordHash}");
                
                var inputHash = HashPassword(request.Password);
                Console.WriteLine($"[DEBUG] Input password hash: {inputHash}");
                Console.WriteLine($"[DEBUG] Hashes match: {inputHash == user.PasswordHash}");
            }

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                Console.WriteLine($"[DEBUG] Login failed: user is null or password mismatch");
                return ServiceResult<LoginResponseDto>.FailureResult("Invalid username or password");
            }

            if (!user.IsActive)
            {
                Console.WriteLine($"[DEBUG] Login failed: account inactive");
                return ServiceResult<LoginResponseDto>.FailureResult("Account is inactive");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var response = new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = _mapper.Map<UserInfoDto>(user)
            };

            // Log audit
            await LogAuditAsync(user.Id, "Login", "Users", user.Id);

            Console.WriteLine($"[DEBUG] Login successful for user: {user.Username}");
            return ServiceResult<LoginResponseDto>.SuccessResult(response, "Login successful");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Login exception: {ex.Message}");
            Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
            return ServiceResult<LoginResponseDto>.FailureResult($"Login failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult<LoginResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // Check if username exists
            if (await _unitOfWork.Users.UsernameExistsAsync(request.Username))
            {
                return ServiceResult<LoginResponseDto>.FailureResult("Username already exists");
            }

            // Check if employee exists and is not already linked
            if (request.EmployeeId.HasValue)
            {
                var existingUser = await _unitOfWork.Users.GetByEmployeeIdAsync(request.EmployeeId.Value);
                if (existingUser != null)
                {
                    return ServiceResult<LoginResponseDto>.FailureResult("Employee already has an account");
                }

                var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId.Value);
                if (employee == null)
                {
                    return ServiceResult<LoginResponseDto>.FailureResult("Employee not found");
                }
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = HashPassword(request.Password);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var response = new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = _mapper.Map<UserInfoDto>(user)
            };

            return ServiceResult<LoginResponseDto>.SuccessResult(response, "Registration successful");
        }
        catch (Exception ex)
        {
            return ServiceResult<LoginResponseDto>.FailureResult($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ChangePasswordAsync(int userId, ChangePasswordDto request)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult.FailureResult("User not found");
            }

            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return ServiceResult.FailureResult("Current password is incorrect");
            }

            user.PasswordHash = HashPassword(request.NewPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await LogAuditAsync(userId, "ChangePassword", "Users", userId);

            return ServiceResult.SuccessResult("Password changed successfully");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Password change failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        // TODO: Implement refresh token logic with database storage
        await Task.CompletedTask;
        return ServiceResult<LoginResponseDto>.FailureResult("Refresh token not implemented yet");
    }

    public async Task<ServiceResult> LogoutAsync(int userId)
    {
        try
        {
            await LogAuditAsync(userId, "Logout", "Users", userId);
            return ServiceResult.SuccessResult("Logout successful");
        }
        catch (Exception ex)
        {
            return ServiceResult.FailureResult($"Logout failed: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyHere123456789012"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("EmployeeId", user.EmployeeId?.ToString() ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "HRMSystem",
            audience: _configuration["Jwt:Audience"] ?? "HRMClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task LogAuditAsync(int userId, string action, string tableName, int recordId)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            TableName = tableName,
            RecordId = recordId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AuditLogs.AddAsync(auditLog);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion
}
