# Isolated Package Feeds

- [About Isolated Package Feeds](#about-isolated-package-feeds)
- [Projects in solution](#projects-in-solution)
- [Solution Development Dependencies](#solution-development-dependencies)
  - [Required APIs](#required-apis)
  - [Required / Strongly Suggested VS Code Extensions](#required--strongly-suggested-vs-code-extensions)
  - [Suggested VS Code Extensions](#suggested-vs-code-extensions)
- [References](#references)
- [Package Sources](#package-sources)

## About Isolated Package Feeds

The goal of this project is to develop content feed provders for environments that are isolated from the public internet, which caches and synchronizes content from public feed providers.

The initial scope is to create NuGet and NPM Package Feed services as well as content from CDN providers.

[Click here to view GitHub project](https://github.com/users/lerwine/projects/4)

## Projects in solution

- [IsolatedPackageFeeds.Shared](./IsolatedPackageFeeds.Shared/README.md) - Class library shared by other projects.
- [IsolatedPackageFeeds.Shared.Tests](./IsolatedPackageFeeds.Shared.Tests/README.md) - Unit tests for the `IsolatedPackageFeeds.Shared` application.
- [CdnGetter](./CdnGetter/README.md) - Console application to manage Local NuGet Feed of libraries hosted by upstream content delivery networks.
- [CdnGetter.UnitTests](./CdnGetter.UnitTests/README.md) - Unit tests for the `CdnGetter` application.
- [CdnServer](./CdnServer/README.md) - CDN website for hosting libraries downloaded by the `CdnGetter` application.
- [NuGetPuller](./NuGetPuller/README.md) - Class library for managing local NuGet repositories and retrieve packages from an upstream NuGet repository.
- [NuGetPuller.CLI](./NuGetPuller.CLI/README.md) - Console application to manage Local NuGet Feed of packages hosted by an upstream NuGet repository.
- [NuGetPuller.CLI](./NuGetPuller.Win/README.md) - GUI application to manage Local NuGet Feed of packages hosted by an upstream NuGet repository.
- [NuGetPuller.UnitTests](./NuGetPuller.UnitTests/README.md) - Unit tests for `NuGetPuller` application.

## Solution Development Dependencies

### Required APIs

| Installer                                                                     | Dev Container Feature                                                                                                   |
|-------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------|
| [DotNet Core 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) | [devcontainers/features/dotnet](https://github.com/devcontainers/features/tree/main/src/dotnet)                         |
| [PowerShell Core](https://github.com/powershell/powershell)                   | [devcontainers/features/powershell](https://github.com/devcontainers/features/tree/main/src/powershell)                 |
| [SQLite](https://www.sqlite.org/download.html)                                | [warrenbuckley/codespace-features/sqlite](https://github.com/warrenbuckley/codespace-features/tree/main/src/sqlite)     |
| [Node.js](https://nodejs.org/en/download/current)                             | [devcontainers/features/node](https://github.com/devcontainers/features/blob/main/src/node)                             |
| [TypeScript](https://www.npmjs.com/package/typescript)                        | [devcontainers-contrib/features/typescript](https://github.com/devcontainers-contrib/features/tree/main/src/typescript) |

### Required / Strongly Suggested VS Code Extensions

- [.NET Core Add Reference](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.add-reference)
- [.NET Core User Secrets](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.user-secrets)
- [Better Comments](https://marketplace.visualstudio.com/items?itemName=aaron-bond.better-comments)
- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- [Git Project Manager](https://marketplace.visualstudio.com/items?itemName=felipecaputo.git-project-manager)
- [JavaScript and TypeScript Nightly](https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-typescript-next)
- [MSBuild project tools](https://marketplace.visualstudio.com/items?itemName=tintoy.msbuild-project-tools)
- [NuGet Gallery](https://marketplace.visualstudio.com/items?itemName=patcx.vscode-nuget-gallery)
- [NuGet Package Manager](https://marketplace.visualstudio.com/items?itemName=jmrog.vscode-nuget-package-manager)
- [PowerShell](https://marketplace.visualstudio.com/items?itemName=ms-vscode.powershell)
- [SQLite](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite)
- [SQLTools SQLite](https://marketplace.visualstudio.com/items?itemName=mtxr.sqltools-driver-sqlite)
- [SQLTools](https://marketplace.visualstudio.com/items?itemName=mtxr.sqltools)
- [Test Adapter Converter](https://marketplace.visualstudio.com/items?itemName=ms-vscode.test-adapter-converter)
- [Test Explorer UI](https://marketplace.visualstudio.com/items?itemName=hbenl.vscode-test-explorer)
- [TODO Highlight](https://marketplace.visualstudio.com/items?itemName=wayou.vscode-todo-highlight)
- [Trx viewer](https://marketplace.visualstudio.com/items?itemName=scabana.trxviewer)
- [TypeScript + Webpack Problem Matchers](https://marketplace.visualstudio.com/items?itemName=amodio.tsl-problem-matcher)

### Suggested VS Code Extensions

- [.NET Core EditorConfig Generator](https://marketplace.visualstudio.com/items?itemName=doggy8088.netcore-editorconfiggenerator)
- [.NET Core Test Explorer](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer)
- [.NET Core Tools](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet)
- [Angular 1.x snippets for Visual Studio Code using John Papa AngularJS style guide](https://marketplace.visualstudio.com/items?itemName=lperdomo.angular1-code-snippets-johnpapastyle)
- [AngularJS 1.5+ TypeScript Snippets](https://marketplace.visualstudio.com/items?itemName=jimmychandra.angularjs-1-5--typescript-snippets)
- [AngularJS Component Generator](https://marketplace.visualstudio.com/items?itemName=ohutsulyak.angularjs-component-generator)
- [AngularJs ngDoc Generator](https://marketplace.visualstudio.com/items?itemName=Luis.angularjs-ngdoc-generator)
- [AngularJS Template Autocomplete](https://marketplace.visualstudio.com/items?itemName=michaelisom.angularjs-template-autocomplete)
- [ASP.NET Core Switcher](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.asp-net-core-switcher)
- [Auto Close Tag](https://marketplace.visualstudio.com/items?itemName=formulahendry.auto-close-tag)
- [Bookmarks](https://marketplace.visualstudio.com/items?itemName=alefragnani.Bookmarks)
- [C# Extensions](https://marketplace.visualstudio.com/items?itemName=kreativ-software.csharpextensions)
- [C# Namespace Autocompletion](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.namespace)
- [C# Snippets](https://marketplace.visualstudio.com/items?itemName=jorgeserrano.vscode-csharp-snippets)
- [C# to TypeScript](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.csharp-to-typescript)
- [C# XML Documentation Comments](https://marketplace.visualstudio.com/items?itemName=k--kato.docomment)
- [Complete JSDoc Tags](https://marketplace.visualstudio.com/items?itemName=HookyQR.JSDocTagComplete)
- [Dev Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)
- [devmate](https://marketplace.visualstudio.com/items?itemName=AutomatedSoftwareTestingGmbH.devmate)
- [DGMLViewer](https://marketplace.visualstudio.com/items?itemName=coderAllan.vscode-dgmlviewer)
- [Document This](https://marketplace.visualstudio.com/items?itemName=oouo-diogo-perdigao.docthis)
- [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)
- [Essential ASP.NET Core Snippets](https://marketplace.visualstudio.com/items?itemName=doggy8088.netcore-snippets)
- [File Downloader](https://marketplace.visualstudio.com/items?itemName=mindaro-dev.file-downloader)
- [Git Extension Pack](https://marketplace.visualstudio.com/items?itemName=donjayamanne.git-extension-pack)
- [Git History](https://marketplace.visualstudio.com/items?itemName=donjayamanne.githistory)
- [GitHub Actions](https://marketplace.visualstudio.com/items?itemName=github.vscode-github-actions)
- [GitHub Pull Requests and Issues](https://marketplace.visualstudio.com/items?itemName=GitHub.vscode-pull-request-github)
- [gitignore](https://marketplace.visualstudio.com/items?itemName=codezombiech.gitignore)
- [GitLens — Git supercharged](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens)
- [HTML Boilerplate](https://marketplace.visualstudio.com/items?itemName=sidthesloth.html5-boilerplate)
- [HTML CSS Support](https://marketplace.visualstudio.com/items?itemName=ecmel.vscode-html-css)
- [IntelliCode API Usage Examples](https://marketplace.visualstudio.com/items?itemName=VisualStudioExptTeam.intellicode-api-usage-examples)
- [IntelliCode](https://marketplace.visualstudio.com/items?itemName=VisualStudioExptTeam.vscodeintellicode)
- [IntelliSense for CSS class names in HTML](https://marketplace.visualstudio.com/items?itemName=Zignd.html-css-class-completion)
- [JavaScript (ES6) code snippets](https://marketplace.visualstudio.com/items?itemName=xabikos.JavaScriptSnippets)
- [json2ts](https://marketplace.visualstudio.com/items?itemName=GregorBiswanger.json2ts)
- [LibMan Tools](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.libman)
- [Markdown All in One](https://marketplace.visualstudio.com/items?itemName=yzhang.markdown-all-in-one)
- [Markdown Preview Enhanced](https://marketplace.visualstudio.com/items?itemName=shd101wyy.markdown-preview-enhanced)
- [Markdown](https://marketplace.visualstudio.com/items?itemName=starkwang.markdown)
- [markdownlint](https://marketplace.visualstudio.com/items?itemName=DavidAnson.vscode-markdownlint)
- [Move TS - Move TypeScript files and update relative imports](https://marketplace.visualstudio.com/items?itemName=stringham.move-ts)
- [ng1.5 components utility](https://marketplace.visualstudio.com/items?itemName=ipatalas.vscode-angular-components-intellisense)
- [npm Intellisense](https://marketplace.visualstudio.com/items?itemName=christian-kohler.npm-intellisense)
- [Open in GitHub, Bitbucket, Gitlab, VisualStudio.com !](https://marketplace.visualstudio.com/items?itemName=ziyasal.vscode-open-in-github)
- [Paste JSON as Code (Refresh)](https://marketplace.visualstudio.com/items?itemName=doggy8088.quicktype-refresh)
- [Paste JSON as Code](https://marketplace.visualstudio.com/items?itemName=quicktype.quicktype)
- [Path Intellisense](https://marketplace.visualstudio.com/items?itemName=christian-kohler.path-intellisense)
- [Peek Hidden Files](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.toggle-hidden)
- [Project Manager](https://marketplace.visualstudio.com/items?itemName=alefragnani.project-manager)
- [quickstart](https://marketplace.visualstudio.com/items?itemName=lolkush.quickstart)
- [Rainbow CSV](https://marketplace.visualstudio.com/items?itemName=mechatroner.rainbow-csv)
- [ResXpress](https://marketplace.visualstudio.com/items?itemName=PrateekMahendrakar.resxpress)
- [Run Terminal Command](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.terminal-commands)
- [SVG](https://marketplace.visualstudio.com/items?itemName=jock.svg)
- [TypeScript Extension Pack](https://marketplace.visualstudio.com/items?itemName=loiane.ts-extension-pack)
- [TypeScript Importer](https://marketplace.visualstudio.com/items?itemName=pmneo.tsimporter)
- [Version Lens](https://marketplace.visualstudio.com/items?itemName=pflannery.vscode-versionlens)
- [VS Code .*proj](https://marketplace.visualstudio.com/items?itemName=jRichardeau.vscode-vsproj)
- [VS Code .njsproj](https://marketplace.visualstudio.com/items?itemName=berkansivri.vscode-njsproj)
- [vscode-icons](https://marketplace.visualstudio.com/items?itemName=vscode-icons-team.vscode-icons)
- [vscode-proto3](https://marketplace.visualstudio.com/items?itemName=zxh404.vscode-proto3)
- [Xml Complete](https://marketplace.visualstudio.com/items?itemName=rogalmic.vscode-xml-complete)
- [XML Toolkit](https://marketplace.visualstudio.com/items?itemName=SAPOSS.xml-toolkit)
- [XML Tools](https://marketplace.visualstudio.com/items?itemName=DotJoshJohnson.xml)
- [XML](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-xml)

## References

General development reference links

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

## Package Sources

Following is a list of package sources it would be desirable to mirror for offline use:

- [Visual Studio Marketplace](https://marketplace.visualstudio.com/vscode)
- [NuGet](./NuGetPuller/README.md#nuget)
- [NPM](https://www.npmjs.com)
  - [Verdaccio](https://www.npmjs.com/package/verdaccio) ([GitHub Repository](https://github.com/verdaccio/verdaccio))
    - [How to set up a free private npm registry… for Windows | by Andrew Anderson | Medium](https://medium.com/@Anderson7301/how-to-set-up-a-free-private-npm-registry-for-windows-f532c6a381ce): Uses Python.
  - [local-npm](https://www.npmjs.com/package/local-npm)
  - [Create your own NPM private feed with Azure DevOps | by Dion van Velde | Medium](https://qdraw.medium.com/create-your-own-npm-private-feed-with-azure-devops-54e02b81a10e)
    - [qdraw/azure-devops-private-npm-artifacts-feed-demo: Demo](https://github.com/qdraw/azure-devops-private-npm-artifacts-feed-demo/)
  - [Private npm modules » Debuggable - Node.js Consulting](http://debuggable.com/posts/private-npm-modules:4e68cc7d-1ac4-42d9-995a-343dcbdd56cb)
  - [registry | npm Docs](https://docs.npmjs.com/cli/v9/using-npm/registry)
    - [npm Blog Archive: New npm Registry Architecture](https://blog.npmjs.org/post/75707294465/new-npm-registry-architecture)
  - [sinopia - npm](https://www.npmjs.com/package/sinopia)
  - Alternatives:
    - [npm packages in the Package Registry | GitLab](https://docs.gitlab.com/ee/user/packages/npm_registry/)
- [CDNs](./CdnGetter/README.md#references-and-links)
