// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
#pragma warning disable CS8618
    internal static IHost Host;
#pragma warning restore CS8618

    private static void Main(string[] args)
    {
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureServices((hbContext, services) => {
                services.AddHostedService<CdnSync.CdnSyncService>();
                IConfigurationSection section = hbContext.Configuration.GetSection(nameof(CdnSync));
                string? databaseFilePath = section.GetValue<string>(CdnSync.CdnSyncDb.SETTINGS_KEY_DbFile);
                if (string.IsNullOrWhiteSpace(databaseFilePath))
                    databaseFilePath = Path.Combine(hbContext.HostingEnvironment.ContentRootPath, CdnSync.CdnSyncDb.DEFAULT_DbFile);
                else
                    databaseFilePath = Path.GetFullPath(Path.IsPathFullyQualified(databaseFilePath) ? databaseFilePath : Path.Combine(hbContext.HostingEnvironment.ContentRootPath, databaseFilePath));
                services.AddDbContext<CdnSync.CdnSyncDb>(opt =>
                opt.UseSqlite(new SqliteConnectionStringBuilder
                {
                    DataSource = databaseFilePath,
                    ForeignKeys = true,
                    Mode = File.Exists(databaseFilePath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate
                }.ConnectionString));
                
                services.Configure<CdnSync.SettingsSections.CdnSyncSettings>(section.GetSection(nameof(CdnSync.SettingsSections.CdnSyncSettings)));
                services.AddSingleton<CdnSync.CdnJsSyncService>();
            })
            .Build();
        Host.Run();
    }
}