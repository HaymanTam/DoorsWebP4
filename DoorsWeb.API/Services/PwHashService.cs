using DoorsWeb.API.Services.Interfaces;

namespace DoorsWeb.API.Services
{
    // bcrypt password hashing (recovered from the greenfield admin/auth work).
    public class PwHashService : IPwHashService
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
