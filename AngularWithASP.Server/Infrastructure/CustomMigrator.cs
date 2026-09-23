using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AngularWithASP.Server.Infrastructure
{
  public class CustomMigrator
  {
    StoreContext Context;
    string ContentRootPath;
    IConfiguration Configuration;
    DataSeeder Seeder;

    public CustomMigrator(StoreContext context, IHostEnvironment env, IConfiguration config, DataSeeder seeder)
    {
      Context = context;
      ContentRootPath = env.ContentRootPath;
      Configuration = config;
      Seeder = seeder;
    }

    public async Task Execute()
    {
      await Context.Database.MigrateAsync();
    }
  }
}
