using System;
using UserManagement.Data.Entities;
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

    public long UserId { get; set; } = 0;
    public string UserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public UserLogAction Action { get; set; }

    public UserLogListItemViewModel(UserLog log)
    {
        Id = log.Id;
        UserId = log.UserId;
        UserName = log.LastKnownName;
        Message = log.Message;
        Created = log.Created;
        Action = log.Action;
    }
}
