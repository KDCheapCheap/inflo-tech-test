using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state.
    /// </summary>
    /// <param name="isActive">Active State to filter by.</param>
    /// <returns></returns>
    IEnumerable<User> FilterByActive(bool isActive);

    /// <summary>
    /// Returns all users
    /// </summary>
    IEnumerable<User> GetAll();

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="id">ID of the user.</param>
    /// <returns></returns>
    User GetUserById(long id);

    /// <summary>
    /// Creates a user and enters them into the database.
    /// </summary>
    /// <param name="newUser">The newly created <see cref="User"/></param>
    /// <returns>The newly created user from the database.</returns>
    User CreateUser(User newUser);

    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <param name="id">ID of the user to delete.</param>
    void DeleteUser(long id);

    User UpdateUser(User editedUser);
    User GetUserByIdUntracked(long id);
}
