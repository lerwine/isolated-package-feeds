using Microsoft.AspNetCore.Mvc;

namespace CdnHost.Controllers;

[ApiController]
[Route("libraries")]
public class LibrariesController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<LibrariesController> _logger;

    public LibrariesController(ILogger<LibrariesController> logger)
    {
        _logger = logger;
    }

    // Route: /libraries
    [HttpGet]
    public IEnumerable<LibraryListItem> GetLibraries()
    {
        
        return Enumerable.Range(1, 5).Select(index => new LibraryListItem
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    // Route: /libraries/{name}
    [HttpGet("{name:string}")]
    public LibraryByName GetLibraryByName(string name)
    {
        throw new NotImplementedException();
    }

    // Route: /libraries/{name}/{version}
    [HttpGet("{name:string}/{version:string}")]
    public LibraryByName GetLibraryByVersion(string name, string version)
    {
        throw new NotImplementedException();
    }
}
