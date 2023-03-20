// See https://aka.ms/new-console-template for more information
using CdnGet.Config;
using CdnGet.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
    .ConfigureServices((hbContext, services) => {
        services.AddHostedService<MainService>();
        IConfigurationSection section = hbContext.Configuration.GetSection(nameof(CdnGet));
        services.Configure<AppSettings>(section);
        string databaseFilePath = AppSettings.GetDbFileName(section.Get<AppSettings>());
        databaseFilePath = Path.GetFullPath(Path.IsPathRooted(databaseFilePath) ? databaseFilePath : Path.Combine(hbContext.HostingEnvironment.ContentRootPath, databaseFilePath));
        services.AddDbContext<ContentDb>(opt =>
            opt.UseSqlite(new SqliteConnectionStringBuilder
            {
                DataSource = databaseFilePath,
                ForeignKeys = true,
                Mode = File.Exists(databaseFilePath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate
            }.ConnectionString)
        );
        foreach (Type type in ContentGetterAttribute.RemoteUpdateServices.Values.Select(t => t.Type))
            services.AddSingleton(type);
    })
    .Build();
host.Run();
