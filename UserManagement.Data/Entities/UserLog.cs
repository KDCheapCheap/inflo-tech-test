using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UserManagement.Data.Enums;

namespace UserManagement.Data.Entities;

public class UserLog
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public UserLogAction Action { get; set; }

    public string BeforeChange { get; set; } = string.Empty;
    public string AfterChange { get; set; } = string.Empty;
}
