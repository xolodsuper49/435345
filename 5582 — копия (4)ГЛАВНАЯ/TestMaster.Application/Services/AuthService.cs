using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestMaster.Infrastructure.Services;
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    public User? CurrentUser { get; private set; }

    public AuthService(AppDbContext db) => _db = db;

    public async Task<(bool, string)> LoginAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null) return (false, "Користувача не знайдено.");
        if (!PasswordHelper.VerifyPassword(password, user.PasswordHash)) return (false, "Невірний пароль.");
        
        CurrentUser = user;
        return (true, "Вхід успішний.");
    }
    public void Logout() => CurrentUser = null;
}
