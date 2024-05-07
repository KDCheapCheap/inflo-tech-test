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

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

    public User GetUserById(long id) => _dataAccess.GetById<User>(id);
}
