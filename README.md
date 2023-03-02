# isolated-package-feeds

The goal of this project is to develop content feed provders for environments that are isolated from the public internet, which caches and synchronizes content from public feed providers.

The initial scope is to create NuGet and NPM Package Feed services as well as content from CDN providers.

## References

### NuGet

- [Overview of Hosting Your Own NuGet Feeds | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Using NuGet.Server to Host NuGet Feeds | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/overview)
  - [Setting up Local NuGet Feeds | Microsoft Learn](https://learn.microsoft.com/en-au/nuget/hosting-packages/local-feeds)
- [NuGet Command-Line Interface (CLI) Reference | Microsoft Learn](https://learn.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference?source=recommendations)
- [Creating a local NuGet repository for offline development | Codurance](https://www.codurance.com/publications/2015/05/04/creating-a-local-nuget-repository)
- [Installing a .nupkg on an offline machine | Nathan Manzi's Blog](https://nmanzi.com/blog/installing-nupkg-offline)

#### NuGet Alternatives

- [NuGet packages in the Package Registry | GitLab](https://docs.gitlab.com/ee/user/packages/nuget_repository/): May require licensing.
- [svenkle/nuget-server: A stand-alone hosted wrapper of the NuGet.Server package](https://github.com/lerwine/nuget-server): Uses Nuget.Server and has an MSI package. Developer is not a U.S. citizen and does not show prior history in GitHub.
- [NSubstitute](https://nsubstitute.github.io/) ([Creating a local NuGet repository for offline development | Codurance](https://www.codurance.com/publications/2015/05/04/creating-a-local-nuget-repository)): May not work in environments that are never routable to the public internet.

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
