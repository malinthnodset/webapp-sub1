namespace SubApp1.DAL;

using SubApp1.Models;

public interface IUserRepository
{
    Task<bool> Create(User user);
    Task<bool> Update(User user);
    Task<bool> Delete(int id);
    Task<User?> GetUserById(int id);
}