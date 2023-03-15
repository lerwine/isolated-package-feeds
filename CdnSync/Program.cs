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
                services.Configure<CdnSync.SettingsSections.CdnSyncSettings>(section);
                string databaseFilePath = CdnSync.SettingsSections.CdnSyncSettings.GetDbFile(section.Get<CdnSync.SettingsSections.CdnSyncSettings>());
                databaseFilePath = Path.GetFullPath(Path.IsPathRooted(databaseFilePath) ? databaseFilePath : Path.Combine(hbContext.HostingEnvironment.ContentRootPath, databaseFilePath));
                services.AddDbContext<CdnSync.CdnSyncDb>(opt =>
                    opt.UseSqlite(new SqliteConnectionStringBuilder
                    {
                        DataSource = databaseFilePath,
                        ForeignKeys = true,
                        Mode = File.Exists(databaseFilePath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate
                    }.ConnectionString)
                );
                services.AddSingleton<CdnSync.CdnJsSyncService>();
            })
            .Build();
        Host.Run();
    }
}