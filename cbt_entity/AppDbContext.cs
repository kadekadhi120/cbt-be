using Microsoft.EntityFrameworkCore;

namespace cbt.entity;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
   
}