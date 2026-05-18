using System;
using Microsoft.EntityFrameworkCore;
using QMS.Models;
namespace QMS.Data;
public class QmsDbContext : DbContext
{
    // ✅ DATABASE CONNECTION STRING IS HERE
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Data Source=LTIN732543\\SQLEXPRESS;" +
                "Database=QMSDB;" +
                "Integrated Security=True;" +
                "TrustServerCertificate=True;"
            );
        }
    }

    //  Tables
    public DbSet<User> Users { get; set; }
    public DbSet<Projector> Projectors { get; set; }
    public DbSet<Inspection> Inspections { get; set; }
}