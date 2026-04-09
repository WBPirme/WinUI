using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OGAS.Data
{
    public class OGASDbContextFactory : IDesignTimeDbContextFactory<OGASDbContext>, IDbContextFactory<OGASDbContext>
    {
        private readonly IConfiguration _configuration;

        public OGASDbContextFactory()
        {
            // 在设计时和运行时，都尝试加载配置文件
            _configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public OGASDbContext CreateDbContext()
        {
            return CreateDbContext(null);
        }

        public OGASDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OGASDbContext>();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new OGASDbContext(optionsBuilder.Options);
        }
    }
}
