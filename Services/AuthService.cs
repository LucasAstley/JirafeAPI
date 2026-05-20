using System.Security.Cryptography;
using System.Text;
using JirafeAPI.DTOs;
using JirafeAPI.Entities;
using JirafeAPI.Repositories;
using JirafeAPI.Utilities;

namespace JirafeAPI.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtHelper _jwtHelper;

    public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, JwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<UserLoginResponse?> RegisterAsync(UserRegisterRequest request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
            return null;

        if (await _userRepository.UsernameExistsAsync(request.Username))
            return null;

        var user = new Entities.User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var accessToken = _jwtHelper.GenerateAccessToken(user.Id, user.Username, user.Email, user.Role);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return new UserLoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<UserLoginResponse?> LoginAsync(UserLoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return null;

        var accessToken = _jwtHelper.GenerateAccessToken(user.Id, user.Username, user.Email, user.Role);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return new UserLoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            return null;

        var user = await _userRepository.GetByIdAsync(refreshTokenEntity.UserId);
        if (user == null)
            return null;

        var newAccessToken = _jwtHelper.GenerateAccessToken(user.Id, user.Username, user.Email, user.Role);
        var newRefreshToken = GenerateRefreshToken();

        refreshTokenEntity.IsRevoked = true;
        await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(int userId)
    {
        await _refreshTokenRepository.RevokeAllByUserIdAsync(userId);
        await _refreshTokenRepository.SaveChangesAsync();
    }

    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

