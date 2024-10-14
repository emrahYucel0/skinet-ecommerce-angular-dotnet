using System;
using System.Linq.Expressions;
using System.Reflection;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class StoreContext : DbContext
{
    protected IConfiguration Configuration { get; set; }
    public DbSet<Product> Products { get; set; }

    public StoreContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
        // Database.EnsureCreated();

        // Eğer migration kullanılıyorsa,
    // Database.Migrate();
    // EnsureCreated, migration gerektirmeyen veritabanı işlemleri için
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}
