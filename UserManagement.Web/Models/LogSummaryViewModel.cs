using UserManagement.Data.Entities;

namespace UserManagement.Web.Models;

public class LogSummaryViewModel
{
    public UserLog Log { get; set; } = new UserLog();
}
