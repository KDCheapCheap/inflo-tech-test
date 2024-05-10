using System.Linq;
using UserManagement.Data.Entities;
using UserManagement.Data;
using UserManagement.Services.Implementations;

namespace UserManagement.Services.Tests;

public class UserLoggingServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();

    private UserLoggingService CreateService() => new UserLoggingService(_dataContext.Object);

    [Fact]
    public void GetAllLogs_WhenContextReturnsLogs_MustReturnLogs()
    {
        // Arrange
        var service = CreateService();
        var logs = SetupLogs();

        // Act
        var result = service.GetAllLogs();

        // Assert
        result.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public void GetAllLogsForUser_WhenContextReturnsLogs_MustReturnLogsForUser()
    {
        // Arrange
        var service = CreateService();
        var userId = 1;
        var logs = SetupLogs().Where(x => x.UserId == userId).ToList();

        // Act
        var result = service.GetAllLogsForUser(userId);

        // Assert
        result.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public void GetLogById_WhenContextReturnsLog_MustReturnLog()
    {
        // Arrange
        var service = CreateService();
        var logId = 1;
        var log = new UserLog { Id = logId, UserId = 1, Action = Data.Enums.UserLogAction.Add };
        _dataContext.Setup(s => s.GetById<UserLog>(logId)).Returns(log);

        // Act
        var result = service.GetLogById(logId);

        // Assert
        result.Should().BeEquivalentTo(log);
    }

    [Fact]
    public void CreateLogEntry_WhenCalled_MustCreateLogEntry()
    {
        // Arrange
        var service = CreateService();
        var log = new UserLog { UserId = 1, Action = Data.Enums.UserLogAction.Add };

        // Act
        service.CreateLogEntry(log);

        // Assert
        _dataContext.Verify(s => s.Create(log), Times.Once);
    }

    [Fact]
    public void DeleteLogEntry_WhenCalled_MustDeleteLogEntry()
    {
        // Arrange
        var service = CreateService();
        var logId = 1;
        var logToDelete = new UserLog { Id = logId, UserId = 1, Action = Data.Enums.UserLogAction.Add };
        _dataContext.Setup(s => s.GetById<UserLog>(logId)).Returns(logToDelete);

        // Act
        service.DeleteLogEntry(logId);

        // Assert
        _dataContext.Verify(s => s.Delete(logToDelete), Times.Once);
    }

    private IQueryable<UserLog> SetupLogs()
    {
        var logs = new[]
        {
            new UserLog { Id = 1, UserId = 1, Action = Data.Enums.UserLogAction.Add },
            new UserLog { Id = 2, UserId = 2, Action =  Data.Enums.UserLogAction.Edit },
            new UserLog { Id = 3, UserId = 1, Action =  Data.Enums.UserLogAction.Delete }
        }.AsQueryable();

        _dataContext.Setup(s => s.GetAll<UserLog>()).Returns(logs);

        return logs;
    }
}

