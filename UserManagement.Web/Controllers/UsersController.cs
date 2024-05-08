using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    [Route("List")]
    public ViewResult List()
    {
        var items = _userService.GetAll().Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("List/{active}")]
    public ViewResult List(bool active)
    {
        var items = _userService.FilterByActive(active).Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("NewUser")]
    [ActionName("CreateUser")]
    public ViewResult CreateUserPage()
    {
        return View();
    }

    [HttpPost]
    [Route("CreateUser")]
    [ActionName("CreateUser")]
    public IActionResult CreateUser(User newUser)
    {
        try
        {
            var createdUser = _userService.CreateUser(newUser);

            return RedirectToAction("View", new { id = createdUser.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Creating User. Message: {ex.Message} Stack trace: {ex.StackTrace}");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("View/{userId}")]
    [ActionName("UserSummary")]
    public ViewResult ViewUser(long userId)
    {
        var user = _userService.GetUserById(userId);

        var userModel = new UserSummaryViewModel
        {
            user = new UserListItemViewModel
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive
            }
        };

        return View(userModel);
    }

    [HttpDelete]
    [Route("Delete/{userId}")]
    [ActionName("DeleteUser")]
    public IActionResult DeleteUser(long userId)
    {
        try
        {
            _userService.DeleteUser(userId);

            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user. Message: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}
