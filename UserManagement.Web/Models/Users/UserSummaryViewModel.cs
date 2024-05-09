using UserManagement.Data.Entities;

namespace UserManagement.Web.Models.Users;

public class UserSummaryViewModel
{
    public UserListItemViewModel user = new();
    public List<UserLog> userLogs = new List<UserLog>();
}
