using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserManagement.Data.Entities;
using UserManagement.Data.Enums;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<ILogger<UsersController>> _loggerMock = new();
    private readonly Mock<IUserLoggingService> _userLoggingServiceMock = new();

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

        _userServiceMock
            .Setup(s => s.GetAll())
            .Returns(users);

        return users;
    }

    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_ReturnsViewResult_WithAllUsers()
    {
        // Arrange
        var users = new List<User>
            {
                new User { Id = 1, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true },
                new User { Id = 2, Forename = "Jane", Surname = "Smith", Email = "jane.smith@example.com", IsActive = false }
            };
        _userServiceMock.Setup(x => x.GetAll()).Returns(users);
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.List() as ViewResult;
        var model = result?.ViewData.Model as UserListViewModel;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(users.Count, model.Items.Count);
        Assert.True(model.Items.All(u => users.Any(au => au.Id == u.Id)));
    }

    [Fact]
    public void List_ReturnsViewResult_WithFilteredUsers()
    {
        // Arrange
        var users = new List<User>
            {
                new User { Id = 1, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true },
                new User { Id = 2, Forename = "Jane", Surname = "Smith", Email = "jane.smith@example.com", IsActive = false }
            };
        var activeUsers = users.Where(u => u.IsActive).ToList();
        _userServiceMock.Setup(x => x.FilterByActive(true)).Returns(activeUsers);
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.List(true) as ViewResult;
        var model = result?.ViewData.Model as UserListViewModel;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(activeUsers.Count, model.Items.Count);
        Assert.True(model.Items.All(u => activeUsers.Any(au => au.Id == u.Id)));
    }

    [Fact]
    public void CreateUser_ReturnsRedirectToActionResult_OnSuccessfulCreation()
    {
        // Arrange
        var newUser = new User { Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        var createdUser = new User { Id = 1, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        _userServiceMock.Setup(x => x.CreateUser(newUser)).Returns(createdUser);
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.CreateUser(newUser) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("View", result.ActionName);
        Assert.Equal(createdUser.Id, result.RouteValues?["id"]);
    }

    [Fact]
    public void ViewUser_ReturnsViewResult_WithUserSummaryViewModel()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        _userServiceMock.Setup(x => x.GetUserById(userId)).Returns(user);
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.ViewUser(userId) as ViewResult;
        var model = result?.ViewData.Model as UserSummaryViewModel;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(userId, model.user.Id);
        Assert.Equal(user.Forename, model.user.Forename);
        Assert.Equal(user.Surname, model.user.Surname);
        Assert.Equal(user.Email, model.user.Email);
        Assert.Equal(user.DateOfBirth, model.user.DateOfBirth);
        Assert.Equal(user.IsActive, model.user.IsActive);
    }

    [Fact]
    public void EditUserPage_ReturnsViewResult_WithUserToEdit()
    {
        // Arrange
        var userId = 1;
        var userToEdit = new User { Id = userId, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        _userServiceMock.Setup(x => x.GetUserById(userId)).Returns(userToEdit);
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.EditUserPage(userId) as ViewResult;
        var model = result?.ViewData.Model as User;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(userId, model.Id);
        Assert.Equal(userToEdit.Forename, model.Forename);
        Assert.Equal(userToEdit.Surname, model.Surname);
        Assert.Equal(userToEdit.Email, model.Email);
        Assert.Equal(userToEdit.DateOfBirth, model.DateOfBirth);
        Assert.Equal(userToEdit.IsActive, model.IsActive);
    }

    [Fact]
    public void EditUser_ReturnsRedirectToActionResult_AfterEditingUser()
    {
        // Arrange
        var editedUser = new User { Id = 1, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        var userBeforeChange = new User { Id = 1, Forename = "Jane", Surname = "Smith", Email = "jane.smith@example.com", IsActive = false };
        var userId = editedUser.Id;
        var userJsonBeforeChange = JsonConvert.SerializeObject(userBeforeChange);
        _userServiceMock.Setup(x => x.GetUserByIdUntracked(userId)).Returns(userBeforeChange);
        _userServiceMock.Setup(x => x.UpdateUser(editedUser)).Returns(editedUser);
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.EditUser(editedUser) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("View", result.ActionName);
        Assert.Equal(editedUser.Id, result.RouteValues?["id"]);

        // Verify log creation
        _userLoggingServiceMock.Verify(x => x.CreateLogEntry(It.Is<UserLog>(log =>
            log.UserId == editedUser.Id &&
            log.LastKnownName == $"{editedUser.Forename} {editedUser.Surname}" &&
            log.Action == UserLogAction.Edit &&
            log.Message == $"Edited User: {editedUser.Id}" &&
            log.BeforeChange == userJsonBeforeChange &&
            log.AfterChange == JsonConvert.SerializeObject(editedUser)
        )), Times.Once);
    }

    [Fact]
    public void DeleteUser_ReturnsRedirectToActionResult_AfterDeletingUser()
    {
        // Arrange
        var userId = 1;
        var userBeforeDeletion = new User { Id = userId, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        var userToDelete = new User { Id = userId, Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        var logToDelete = new UserLog { Id = 1, UserId = userId, Action = UserLogAction.Delete };
        _userServiceMock.Setup(x => x.GetUserById(userId)).Returns(userBeforeDeletion);
        _userServiceMock.Setup(x => x.DeleteUser(userId));
        _userLoggingServiceMock.Setup(x => x.CreateLogEntry(It.IsAny<UserLog>()));
        _userLoggingServiceMock.Setup(x => x.GetLogById(logToDelete.Id)).Returns(logToDelete);
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.DeleteUser(userId) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("List", result.ActionName);

        // Verify log creation
        _userLoggingServiceMock.Verify(x => x.CreateLogEntry(It.Is<UserLog>(log =>
            log.UserId == userBeforeDeletion.Id &&
            log.LastKnownName == $"{userBeforeDeletion.Forename} {userBeforeDeletion.Surname}" &&
            log.Action == UserLogAction.Delete &&
            log.Message == $"Deleted User {userBeforeDeletion.Id}" &&
            log.BeforeChange == JsonConvert.SerializeObject(userBeforeDeletion) &&
            log.AfterChange == string.Empty
)),         Times.Once);
    }

    [Fact]
    public void CreateUser_ReturnsBadRequest_WhenExceptionOccurs()
    {
        // Arrange
        var newUser = new User { Forename = "John", Surname = "Doe", Email = "john.doe@example.com", IsActive = true };
        var exceptionMessage = "Error creating user.";
        _userServiceMock.Setup(x => x.CreateUser(newUser)).Throws(new Exception(exceptionMessage));
        var controller = new UsersController(_userServiceMock.Object, _loggerMock.Object, _userLoggingServiceMock.Object);

        // Act
        var result = controller.CreateUser(newUser) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(exceptionMessage, result.Value);
    }
}
