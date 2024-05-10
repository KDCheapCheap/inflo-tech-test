using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Implementations;

public class UserLoggingService : IUserLoggingService
{
    private readonly IDataContext _dataAccess;
    public UserLoggingService(IDataContext dataAccess) => _dataAccess = dataAccess;

    public IEnumerable<UserLog> GetAllLogs() => _dataAccess.GetAll<UserLog>();

    /// <summary>
    /// Gets all logs for a specific user.
    /// </summary>
    /// <param name="userId">The Id of the user to get logs for.</param>
    /// <returns>A List of Logs pertaining to the user, if found.</returns>
    public List<UserLog> GetAllLogsForUser(long userId)
    {
        return _dataAccess.GetAll<UserLog>().Where(x => x.UserId == userId).ToList();
    }

    public UserLog GetLogById(long logId)
    {
        return _dataAccess.GetById<UserLog>(logId);
    }

    public void CreateLogEntry(UserLog log)
    {
        try
        {
            _dataAccess.Create(log);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating log entry. Message: {ex.Message}");
            throw;
        }
    }

    public void DeleteLogEntry(long logId)
    {
        UserLog logToDelete = GetLogById(logId);

        if (logToDelete == null)
        {
            Console.WriteLine($"Cannot find Log with ID: {logId}");
            throw new KeyNotFoundException($"Cannot find Log with ID: {logId}");
        }

        try
        {
            _dataAccess.Delete(logToDelete);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error when deleting Log. Message: {ex.Message}");
        }
    }
}
