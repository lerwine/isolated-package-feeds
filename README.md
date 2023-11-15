# isolated-package-feeds

The goal of this project is to develop content feed provders for environments that are isolated from the public internet, which caches and synchronizes content from public feed providers.

The initial scope is to create NuGet and NPM Package Feed services as well as content from CDN providers.

[Click here to view project](https://github.com/users/lerwine/projects/4)

## References

- [SQLite Data Types](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types)
- [SQLite Syntax](https://www.sqlite.org/lang.html)
- [SQLite Database Provider - EF Core](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/?tabs=dotnet-core-cli)
- [EF Core](https://learn.microsoft.com/en-us/ef/core/)
- [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- [Configuration in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [Make HTTP requests with the HttpClient class](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient)
  - [JsonContent Class](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.jsoncontent?view=net-7.0)
  - [StringContent Class](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.stringcontent?view=net-7.0)
- [NUnit.org](https://nunit.org/)
  - [NUnit Documentation Site](https://docs.nunit.org/)
  - [Unit testing C# with NUnit and .NET Core](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit)
  - [Most Complete NUnit Unit Testing Framework Cheat Sheet](https://www.automatetheplanet.com/nunit-cheat-sheet/)
  - [CodeProject: Unit Testing Using NUnit](https://www.codeproject.com/articles/178635/unit-testing-using-nunit)
- [XUnit Testing](https://xunit.net)
- [c# Documentation Comments](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments)

### NPM

- [Verdaccio](https://www.npmjs.com/package/verdaccio) ([GitHub Repository](https://github.com/verdaccio/verdaccio))
  - [How to set up a free private npm registry… for Windows | by Andrew Anderson | Medium](https://medium.com/@Anderson7301/how-to-set-up-a-free-private-npm-registry-for-windows-f532c6a381ce): Uses Python.
- [local-npm](https://www.npmjs.com/package/local-npm)
- [Create your own NPM private feed with Azure DevOps | by Dion van Velde | Medium](https://qdraw.medium.com/create-your-own-npm-private-feed-with-azure-devops-54e02b81a10e)
  - [qdraw/azure-devops-private-npm-artifacts-feed-demo: Demo](https://github.com/qdraw/azure-devops-private-npm-artifacts-feed-demo/)
- [Private npm modules » Debuggable - Node.js Consulting](http://debuggable.com/posts/private-npm-modules:4e68cc7d-1ac4-42d9-995a-343dcbdd56cb)
- [registry | npm Docs](https://docs.npmjs.com/cli/v9/using-npm/registry)
  - [npm Blog Archive: New npm Registry Architecture](https://blog.npmjs.org/post/75707294465/new-npm-registry-architecture)
- [sinopia - npm](https://www.npmjs.com/package/sinopia)

#### NPM Alternatives

- [npm packages in the Package Registry | GitLab](https://docs.gitlab.com/ee/user/packages/npm_registry/)

### Content Delivery Networks

- [cdnjs](https://cdnjs.com/)
  - [API Documentation](https://cdnjs.com/api)
  - [Main Git Repository](https://github.com/lerwine/cdnjs)
    - [Vue/Nuxt website for cdnjs.com](https://github.com/lerwine/static-website)
    - [API server for api.cdnjs.com](https://github.com/lerwine/api-server)
    - [Package configurations](https://github.com/lerwine/packages)
    - [Brand and design assets for cdnjs](https://github.com/lerwine/brand)
- [jsdelivr](https://www.jsdelivr.com/)
  - [API Documentation](https://www.jsdelivr.com/docs/data.jsdelivr.com#overview)
