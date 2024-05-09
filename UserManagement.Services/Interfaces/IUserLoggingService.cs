using System.Collections.Generic;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Domain.Interfaces;
public interface IUserLoggingService
{
    void CreateLogEntry(UserLog log);
    void DeleteLogEntry(long logId);
    IEnumerable<UserLog> GetAllLogs();
    List<UserLog> GetAllLogsForUser(long userId);
}
