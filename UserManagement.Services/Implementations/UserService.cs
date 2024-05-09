using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        List<User> users = _dataAccess.GetAll<User>().ToList();
        List<User> filteredUsers = users.FindAll(x => x.IsActive == isActive);

        return filteredUsers;
    }

    public IEnumerable<User> GetAll()
    {
        return _dataAccess.GetAll<User>();
    }

    public User GetUserById(long id)
    {
        return _dataAccess.GetById<User>(id);
    }

    public User GetUserByIdUntracked(long id)
    {
        return _dataAccess.GetByIdUntracked<User>(id);
    }

    public User CreateUser(User newUser)
    {
        return _dataAccess.Create(newUser);
    }

    public User UpdateUser(User editedUser)
    {
        return _dataAccess.Update(editedUser);
    }

    public void DeleteUser(long id)
    {
        User userToDelete = GetUserById(id);

        if(userToDelete == null)
        {
            throw new KeyNotFoundException("User not found with ID: " + id);
        }

        _dataAccess.Delete(userToDelete);
    }
}
