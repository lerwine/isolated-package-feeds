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
        Console.WriteLine("Hello, World!");
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
        IConfigurationSection section = builder.Configuration.GetSection(nameof(CdnSync));
        builder.Services.Configure<CdnSync.CdnSyncAppSettings>(section);
        string? databaseFilePath = section.Get<CdnSync.CdnSyncAppSettings>()?.DbFile;
        if (string.IsNullOrWhiteSpace(databaseFilePath))
            databaseFilePath = Path.Combine(builder.Environment.ContentRootPath, CdnSync.CdnSyncAppSettings.DEFAULT_DbFile);
        else
            databaseFilePath = Path.GetFullPath(Path.IsPathFullyQualified(databaseFilePath) ? databaseFilePath : Path.Combine(builder.Environment.ContentRootPath, databaseFilePath));
        builder.Services.AddDbContext<CdnSync.CdnSyncDb>(opt =>
            opt.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                ForeignKeys = true,
                Mode = File.Exists(databaseFilePath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate
            }.ConnectionString));
        Host = builder.Build();
    }
}