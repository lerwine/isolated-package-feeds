using CdnGetter.Config;
using CdnGetter.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    internal static IHost Host { get; private set; } = null!;

    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        if (builder.Environment.IsDevelopment())
            builder.Configuration.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), true);
        builder.Logging.AddConsole();
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(CdnGetter)));
        AppSettings.Configure(args, builder.Configuration);
        builder.Services.AddDbContextPool<ContentDb>(options =>
            {
                var dbFile = builder.Configuration.GetSection(nameof(CdnGetter)).Get<AppSettings>()?.DbFile;
                try
                {
                    dbFile = Path.GetFullPath(string.IsNullOrEmpty(dbFile) ? Path.Combine(builder.Environment.ContentRootPath, CdnGetter.Constants.DEFAULT_DbFile) :
                        Path.IsPathFullyQualified(dbFile) || Path.IsPathRooted(dbFile) ? dbFile : Path.Combine(builder.Environment.ContentRootPath, dbFile));
                }
                catch { }
                options.UseSqlite(new SqliteConnectionStringBuilder
                {
                    DataSource = dbFile,
                    ForeignKeys = true,
                    Mode = SqliteOpenMode.ReadWrite
                }.ConnectionString);
            })
            .AddHostedService<MainService>();
        foreach (Type type in ContentGetterAttribute.UpstreamCdnServices.Values.Select(t => t.Type))
            builder.Services.AddSingleton(type);
        Host = builder.Build();
        Host.Run();
    }
}