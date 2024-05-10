using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
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
        };

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        return users;
    }

    [Fact]
    public void CreateUser_ReturnsRedirectToActionResult_WhenModelStateIsValid()
    {
        // Arrange
        var newUser = new User();
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.CreateUser(newUser)).Returns(newUser);
        var loggerMock = new Mock<ILogger<UsersController>>();
        var userLoggingServiceMock = new Mock<IUserLoggingService>();
        var controller = new UsersController(userServiceMock.Object, loggerMock.Object, userLoggingServiceMock.Object);

        // Act
        var result = controller.CreateUser(newUser);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("View", redirectToActionResult.ActionName);
        Assert.Equal(newUser.Id, redirectToActionResult?.RouteValues?["id"]);
    }

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<ILogger<UsersController>> _userLogger = new();
    private readonly Mock<IUserLoggingService> _userLoggingService = new();
    private UsersController CreateController() => new(_userService.Object, _userLogger.Object, _userLoggingService.Object);
}
