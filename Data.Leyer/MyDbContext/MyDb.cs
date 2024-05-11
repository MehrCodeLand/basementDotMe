using Data.Leyer.MyEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Data.Leyer.MyDbContext;

public class MyDb : DbContext
{
    public MyDb(DbContextOptions<MyDb> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permissions> Permissions { get; set; }

}