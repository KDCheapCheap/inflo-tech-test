using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    private List<User> _testUsers = new List<User>
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
        };

    private DbContextOptions<DataContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void GetAll_ReturnsAllEntities()
    {
        // Arrange
        var context = new DataContext();

        context.Set<User>().RemoveRange(context.Set<User>());

        context.AddRange(_testUsers);
        context.SaveChanges();

        // Act
        var result = context.GetAll<User>();

        // Assert
        Assert.Equal(_testUsers.Count, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntitiesAsync()
    {
        // Arrange
        var context = new DataContext();
        context.Set<User>().RemoveRange(context.Set<User>());
        context.AddRange(_testUsers);
        context.SaveChanges();

        // Act
        var result = await context.GetAllAsync<User>();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.

        var entity = new User
        {
            Id = 1,
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        var context = new DataContext();
        context.Set<User>().RemoveRange(context.Set<User>());
        context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = new DataContext();
        var entity = context.GetAll<User>().First();
        context.Delete(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void GetById_ShouldReturnEntityById()
    {
        // Arrange
        long id = 1;
        var context = new DataContext();

        // Act
        var result = context.GetById<User>(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntityByIdAsync()
    {
        // Arrange
        long id = 1;
        var context = new DataContext();

        // Act
        var result = await context.GetByIdAsync<User>(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public void GetByIdUntracked_ShouldReturnUntrackedEntityById()
    {
        // Arrange
        long id = 1;
        var context = new DataContext();

        // Act
        var result = context.GetByIdUntracked<User>(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdUntrackedAsync_ShouldReturnUntrackedEntityByIdAsync()
    {
        // Arrange
        long id = 1;
        var context = new DataContext();

        // Act
        var result = await context.GetByIdUntrackedAsync<User>(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public void Create_ShouldCreateEntity()
    {
        // Arrange
        var user = new User
        {
            Forename = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1980, 1, 1),
            IsActive = true
        };
        var context = new DataContext();

        // Act
        var result = context.Create(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Forename, result.Forename);
        Assert.Equal(user.Surname, result.Surname);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateEntityAsync()
    {
        // Arrange
        var user = new User
        {
            Forename = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1980, 1, 1),
            IsActive = true
        };
        var context = new DataContext();

        // Act
        var result = await context.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Forename, result.Forename);
        Assert.Equal(user.Surname, result.Surname);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public void Update_ShouldUpdateEntity()
    {
        // Arrange
        var context = new DataContext();
        var user = context.GetById<User>(1);
        user.Forename = "Updated";

        // Act
        var result = context.Update(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Forename, result.Forename);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntityAsync()
    {
        // Arrange
        var context = new DataContext();
        var user = await context.GetByIdAsync<User>(1);
        user.Forename = "Updated";

        // Act
        var result = await context.UpdateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Forename, result.Forename);
    }

    [Fact]
    public void Delete_ShouldDeleteEntity()
    {
        // Arrange
        var context = new DataContext();
        context.AddRange(_testUsers);
        context.SaveChanges();

        var user = context.GetById<User>(1);

        // Act
        context.Delete(user);

        // Assert
        var result = context.Users?.FirstOrDefault(u => u.Id == user.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteEntityAsync()
    {
        // Arrange
        var context = new DataContext();
        var user = await context.GetByIdAsync<User>(1);

        // Act
        await context.DeleteAsync(user).ConfigureAwait(false);

        // Assert
        var result = context.Users?.Any(u => u.Id == user.Id);
        Assert.False(result);
    }
}
