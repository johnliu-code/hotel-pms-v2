using Microsoft.EntityFrameworkCore;

namespace HotelPms.Infrastructure.Persistence;

/// <summary>
/// EF Core persistence boundary for Hotel PMS 2.0.
/// Entity mappings are supplied through Infrastructure Fluent API configurations.
/// </summary>
public sealed class HotelPmsDbContext(DbContextOptions<HotelPmsDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HotelPmsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
