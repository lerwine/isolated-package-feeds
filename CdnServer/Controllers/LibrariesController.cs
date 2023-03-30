using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CdnServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CdnServer.Controllers
{
    [ApiController]
    [Route("libraries")]
    public class LibrariesController : ControllerBase
    {
        [HttpGet]
#pragma warning disable CS1998, IDE0060
        public async Task<ActionResult<Collection<LibraryListItem>>> GetAllLibraries(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
#pragma warning restore CS1998, IDE0060
        
        [HttpGet("{name}")]
#pragma warning disable CS1998, IDE0060
        public async Task<ActionResult<LibraryByName>> GetLibraryByName(string name, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
#pragma warning restore CS1998, IDE0060
        
        [HttpGet("{name}/{version}")]
#pragma warning disable CS1998, IDE0060
        public async Task<ActionResult<LibraryByVersion>> GetLibraryByVersion(string name, string version, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
#pragma warning restore CS1998, IDE0060
    }
}