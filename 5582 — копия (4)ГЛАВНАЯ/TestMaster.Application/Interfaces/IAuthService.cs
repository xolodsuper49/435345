public interface IAuthService
{
    User? CurrentUser { get; }
    System.Threading.Tasks.Task<(bool, string)> LoginAsync(string username, string password);
    void Logout();
}
