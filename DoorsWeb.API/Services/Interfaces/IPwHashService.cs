namespace DoorsWeb.API.Services.Interfaces
{
    public interface IPwHashService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
