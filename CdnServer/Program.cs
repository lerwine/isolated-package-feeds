using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
IConfigurationSection section = builder.Configuration.GetSection(nameof(CdnServer));
builder.Services.Configure<CdnServer.CdnServerAppSettings>(section);

CdnServer.CdnServerAppSettings settings = builder.Configuration.Get<CdnServer.CdnServerAppSettings>();
string? databaseFilePath = settings.DbFile;

if (string.IsNullOrWhiteSpace(databaseFilePath))
    databaseFilePath = Path.Combine(builder.Environment.WebRootPath, $"{nameof(CdnServer)}.db");
else
    databaseFilePath = Path.IsPathFullyQualified(databaseFilePath) ? Path.GetFullPath(databaseFilePath) : Path.Combine(builder.Environment.WebRootPath, databaseFilePath);
// app.Logger.LogInformation("Using database {databaseFilePath}", databaseFilePath);
builder.Services.AddDbContext<CdnServer.Services.CdnAppDbContext>(opt =>
    opt.UseSqlite(new SqliteConnectionStringBuilder
    {
        DataSource = string.IsNullOrWhiteSpace(databaseFilePath) ? $"{nameof(CdnServer)}.db" : databaseFilePath,
        ForeignKeys = true,
        Mode = File.Exists(databaseFilePath) ? SqliteOpenMode.ReadWrite : SqliteOpenMode.ReadWriteCreate
    }.ConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}"
);

app.MapFallbackToFile("index.html");

app.Run();
