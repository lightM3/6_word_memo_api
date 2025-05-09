using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WordMemoryApi.Data;
using WordMemoryApi.DTOs;
using WordMemoryApi.Entities;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
        if (exists)
            return false;

        using var hmac = new HMACSHA512();
        var user = new User
        {
            Username = dto.Username,
            PasswordSalt = hmac.Key,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password))
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null) return null;

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

        return computedHash.SequenceEqual(user.PasswordHash) ? user : null;
    }
}
