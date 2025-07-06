using Microsoft.EntityFrameworkCore;
using PersonCRUD.Models;

namespace PersonCRUD.Data;

public class PersonContext : DbContext
{
    public DbSet<PersonModel> People { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=person.sqlite");
        base.OnConfiguring(optionsBuilder);
    }
}