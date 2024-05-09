using System;
using UserManagement.Data.Enums;

namespace UserManagement.Data.Entities;

public class UserLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public UserLogAction Action { get; set; }

    public string BeforeChange { get; set; } = string.Empty;
    public string AfterChange { get; set; } = string.Empty;
}
