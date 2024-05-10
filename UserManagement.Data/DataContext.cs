using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("UserManagement.Data.DataContext");

    protected override void OnModelCreating(ModelBuilder model)
        => model.Entity<User>().HasData(new[]
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
            new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = false },
            new User { Id = 4, Forename = "Memphis", Surname = "Raines", Email = "mraines@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
            new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", Email = "sgodspeed@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
            new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", Email = "himcdunnough@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
            new User { Id = 7, Forename = "Cameron", Surname = "Poe", Email = "cpoe@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = false },
            new User { Id = 8, Forename = "Edward", Surname = "Malus", Email = "emalus@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = false },
            new User { Id = 9, Forename = "Damon", Surname = "Macready", Email = "dmacready@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = false },
            new User { Id = 10, Forename = "Johnny", Surname = "Blaze", Email = "jblaze@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
            new User { Id = 11, Forename = "Robin", Surname = "Feld", Email = "rfeld@example.com", DateOfBirth = new DateTime(1995, 4, 6), IsActive = true },
        });

    public DbSet<User>? Users { get; set; }
    public DbSet<UserLog> UserLogs { get; set; }

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : class
    {
        return await Set<TEntity>().ToListAsync();
    }

    public TEntity GetById<TEntity>(long id) where TEntity : class
    {
        var entity = Set<TEntity>().Find(id);

        if (entity == null)
        {
            throw new Exception("Cannot find user for ID: " +  id);
        }

        return entity;
    }

    public async Task<TEntity> GetByIdAsync<TEntity>(long id) where TEntity : class
    {
        var entity = await Set<TEntity>().FindAsync(id);

        if (entity == null)
        {
            throw new Exception("Cannot find user for ID: " +  id);
        }

        return entity;
    }

    public TEntity GetByIdUntracked<TEntity>(long id) where TEntity : class
    {
        var entity = Set<TEntity>().AsNoTracking().SingleOrDefault(e => EF.Property<long>(e, "Id") == id);

        if (entity == null)
        {
            throw new Exception("Cannot find user for ID: " +  id);
        }

        return entity;
    }

    public async Task<TEntity> GetByIdUntrackedAsync<TEntity>(long id) where TEntity : class
    {
        var entity = await Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(e => EF.Property<long>(e, "Id") == id);

        if (entity == null)
        {
            throw new Exception("Cannot find user for ID: " +  id);
        }

        return entity;
    }

    public TEntity Create<TEntity>(TEntity entity) where TEntity : class
    {
        base.Add(entity);
        SaveChanges();

        return entity;
    }

    public async Task<TEntity> CreateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        var add = await base.AddAsync(entity);
        await SaveChangesAsync();

        return add.Entity;
    }

    public new TEntity Update<TEntity>(TEntity entity) where TEntity : class
    {
        base.Update(entity);
        SaveChanges();

        return entity;
    }

    public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        var update = base.Update(entity);
        await SaveChangesAsync();

        return entity;
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        SaveChanges();
    }

    public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
    {
        base.Remove(entity);
        await SaveChangesAsync();
    }
}
