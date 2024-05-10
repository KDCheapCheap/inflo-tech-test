using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);

    [Fact]
    public void FilterByActive_WhenContextReturnsEntities_MustReturnFilteredEntities()
    {
        // Arrange
        var service = CreateService();
        var activeUsers = SetupUsers(isActive: true);

        // Act
        var result = service.FilterByActive(true);

        // Assert
        result.Should().BeEquivalentTo(activeUsers);
    }

    [Fact]
    public void GetUserById_WhenContextReturnsEntity_MustReturnEntity()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Id = 1, Forename = "John", Surname = "Doe" };
        _dataContext.Setup(s => s.GetById<User>(1)).Returns(user);

        // Act
        var result = service.GetUserById(1);

        // Assert
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public void GetUserByIdUntracked_WhenContextReturnsEntity_MustReturnEntity()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Id = 1, Forename = "John", Surname = "Doe" };
        _dataContext.Setup(s => s.GetByIdUntracked<User>(1)).Returns(user);

        // Act
        var result = service.GetUserByIdUntracked(1);

        // Assert
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public void CreateUser_WhenCalled_MustReturnCreatedUser()
    {
        // Arrange
        var service = CreateService();
        var newUser = new User { Forename = "John", Surname = "Doe", Email = "john.doe@example.com" };
        _dataContext.Setup(s => s.Create(newUser)).Returns(newUser);

        // Act
        var result = service.CreateUser(newUser);

        // Assert
        result.Should().BeEquivalentTo(newUser);
    }

    [Fact]
    public void UpdateUser_WhenCalled_MustReturnUpdatedUser()
    {
        // Arrange
        var service = CreateService();
        var editedUser = new User { Id = 1, Forename = "Updated", Surname = "User" };
        _dataContext.Setup(s => s.Update(editedUser)).Returns(editedUser);

        // Act
        var result = service.UpdateUser(editedUser);

        // Assert
        result.Should().BeEquivalentTo(editedUser);
    }

    [Fact]
    public void DeleteUser_WhenCalled_MustDeleteUser()
    {
        // Arrange
        var service = CreateService();
        var userToDelete = new User { Id = 1, Forename = "John", Surname = "Doe" };
        _dataContext.Setup(s => s.GetById<User>(1)).Returns(userToDelete);

        // Act
        service.DeleteUser(1);

        // Assert
        _dataContext.Verify(s => s.Delete(userToDelete), Times.Once);
    }

    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }
}
