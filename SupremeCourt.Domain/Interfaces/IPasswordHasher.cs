namespace SupremeCourt.Domain.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }
}