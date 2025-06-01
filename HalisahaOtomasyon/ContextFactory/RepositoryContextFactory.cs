using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace HalisahaOtomasyon.ContextFactory
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();


            //.UseSqlServer(configuration.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly("HalisahaOtomasyon"));
            //.UseSqlite(configuration.GetConnectionString("sqliteConnection"), b => b.MigrationsAssembly("HalisahaOtomasyon"));

            var connectionString = configuration.GetConnectionString("MySqlConnection");

            var builder = new DbContextOptionsBuilder<RepositoryContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly("HalisahaOtomasyon"));
                    

            return new RepositoryContext(builder.Options);
        }
    }
}