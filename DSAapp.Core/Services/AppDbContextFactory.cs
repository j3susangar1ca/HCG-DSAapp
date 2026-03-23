using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DSAapp.Core.Services;

namespace DSAapp.Core.Services;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        return new AppDbContext();
    }
}
