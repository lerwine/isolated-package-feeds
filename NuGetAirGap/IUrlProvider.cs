namespace NuGetAirGap;

public interface IUrlProvider
{
    string OriginalString { get; }
    string GetPath();
    bool GetPath(out string path);
    Uri GetUri();
}

