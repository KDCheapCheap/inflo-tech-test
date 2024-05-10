using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using UserManagement.Data.Entities;
using UserManagement.Data.Enums;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Controllers;
using UserManagement.Web.Models;

namespace UserManagement.Data.Tests;

public class LoggingControllerTests
{
    private readonly Mock<IUserLoggingService> _userLoggingServiceMock = new();
    private readonly Mock<IUserService> _userServiceMock = new();

    [Fact]
    public void ViewLogs_ReturnsViewResult_WithUserLogListViewModel()
    {
        // Arrange
        var logs = new List<UserLog>
            {
                new UserLog { Id = 1, UserId = 1, Message = "Log 1", Created = DateTime.Now, Action = UserLogAction.Add },
                new UserLog { Id = 2, UserId = 1, Message = "Log 2", Created = DateTime.Now.AddDays(-1), Action = UserLogAction.Edit },
                new UserLog { Id = 3, UserId = 2, Message = "Log 3", Created = DateTime.Now.AddDays(-2), Action = UserLogAction.Delete }
            };
        _userLoggingServiceMock.Setup(x => x.GetAllLogs()).Returns(logs);
        var controller = new LoggingController(_userLoggingServiceMock.Object, _userServiceMock.Object);

        // Act
        var result = controller.ViewLogs() as ViewResult;
        var model = result?.ViewData.Model as UserLogListViewModel;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(logs.Count, model.Items.Count);
        foreach (var log in logs)
        {
            Assert.Contains(model.Items, item =>
                item.Id == log.Id &&
                item.UserId == log.UserId &&
                item.Message == log.Message &&
                item.Created == log.Created &&
                item.Action == log.Action);
        }
    }

    [Fact]
    public void ViewLogsForUser_ReturnsViewResult_WithUserLogListViewModel()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Forename = "John", Surname = "Doe" };
        var logs = new List<UserLog>
            {
                new UserLog { Id = 1, UserId = userId, Message = "Log 1", Created = DateTime.Now, Action = UserLogAction.Add, LastKnownName = $"{user.Forename} {user.Surname}" },
                new UserLog { Id = 2, UserId = userId, Message = "Log 2", Created = DateTime.Now.AddDays(-1), Action = UserLogAction.Edit, LastKnownName = $"{user.Forename} {user.Surname}" },
            };
        _userServiceMock.Setup(x => x.GetUserById(userId)).Returns(user);
        _userLoggingServiceMock.Setup(x => x.GetAllLogsForUser(userId)).Returns(logs);
        var controller = new LoggingController(_userLoggingServiceMock.Object, _userServiceMock.Object);

        // Act
        var result = controller.ViewLogsForUser(userId) as ViewResult;
        var model = result?.ViewData.Model as UserLogListViewModel;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(logs.Count, model.Items.Count);
        Assert.Equal($"{user.Forename} {user.Surname}", model.UsersName);
        foreach (var log in logs)
        {
            Assert.Contains(model.Items, item =>
                item.Id == log.Id &&
                item.UserId == log.UserId &&
                item.Message == log.Message &&
                item.Created == log.Created &&
                item.Action == log.Action);
        }
    }

    // Add more tests for error handling and edge cases...
}
