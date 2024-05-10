using System;
using UserManagement.Data.Enums;

namespace UserManagement.Web.Models;

public class UserLogListViewModel
{
    public string UsersName { get; set; } = string.Empty;
    public List<UserLogListItemViewModel> Items { get; set; } = new();
}

public class UserLogListItemViewModel
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public UserLogAction Action { get; set; }
}
